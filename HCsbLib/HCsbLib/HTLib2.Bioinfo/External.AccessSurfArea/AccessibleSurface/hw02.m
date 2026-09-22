function hw02()
    clear;
    clc;
    
    pdb = pdbread('2GB1.pdb');
    protein = htlib.pdb.read(pdb,{'atom','heme'});
    protein_atoms_num = size(protein.AtomName,1);
    protein_res_num = max(protein.resSeq);
    
    [pts_sphere, acs_sphere, acs_cutoff] = build_spherepoints();
    pts = cell(protein_atoms_num,1);
    acs = cell(protein_atoms_num,1);
    rds = cell(protein_atoms_num,1);
    ais = cell(protein_atoms_num,1);
    protein.atomRadii = zeros(protein_atoms_num,1);
    for i=1:protein_atoms_num
        % probe sphere
        radii = 1.4;
        % Bondi's van der waals radii
        % http://en.wikipedia.org/wiki/Van_der_Waals_radius
        switch(protein.atomType(i,1))
            case 'H'; radii = radii + 1.20; protein.atomRadii(i) = radii;
            case 'C'; radii = radii + 1.70; protein.atomRadii(i) = radii;
            case 'O'; radii = radii + 1.52; protein.atomRadii(i) = radii;
            case 'N'; radii = radii + 1.55; protein.atomRadii(i) = radii;
            case 'S'; radii = radii + 1.80; protein.atomRadii(i) = radii;
            otherwise; disp(sprintf('wrong atom type(%s) at %d',protein.atomType(i,:),i)); return
        end
        pts{i} = protein.coord(i(ones(500,1)),:) + pts_sphere * radii;
        acs{i} = acs_sphere;
        rds{i} = radii(ones(500,1));
        ais{i} = i(ones(500,1));
    end
    cutoff = 1.4+1.20*acs_cutoff;

    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    disp('solvent accessible surface area is determined by');
    disp(' sum of {4*pi*surf_radius(i)^2/500} for all surface points i of');
    disp(' each atoms, residues, and protein, with assuming that');
    disp(' there are 500 points on each atom sphere with radius');
    disp(' (van der walls rad + prob sphere rad)');

    if(~exist('cache/ptsmg.mat'))
        [ptsmg, acsmg, rdsmg, aismg] = build_pts_acs(1:protein_atoms_num, protein, pts, acs, rds, ais, cutoff);
        save('cache/ptsmg.mat', 'ptsmg');
        save('cache/acsmg.mat', 'acsmg');
        save('cache/rdsmg.mat', 'rdsmg');
        save('cache/aismg.mat', 'aismg');
    end
	load('cache/ptsmg.mat', 'ptsmg');
    load('cache/acsmg.mat', 'acsmg');
    load('cache/rdsmg.mat', 'rdsmg');
    load('cache/aismg.mat', 'aismg');

    idxgrps = build_indexgroup(acsmg);
    surface_ptsmg = ptsmg(idxgrps{1},:);
    surface_acsmg = acsmg(idxgrps{1},idxgrps{1});
    surface_rdsmg = rdsmg(idxgrps{1});
    surface_aismg = aismg(idxgrps{1});
    
    disp('solvent accessible surface area of atoms');
    for i=1:protein_atoms_num
        idxs = find(surface_aismg==i);
        atomSASA  = compute_SASA(surface_rdsmg(idxs));
        atomType  = protein.atomType(i,:);
        disp(sprintf('atom %3d: type(%s), #points(%3d), SASA(%8.4f A^2)', i, atomType, length(idxs), atomSASA));
    end

    disp(sprintf('\n'))
    disp('solvent accessible surface area of residues');
    for i=1:protein_res_num
        atomids = find(protein.resSeq == i);
        idxs = (surface_aismg == atomids(1));
        for j=2:length(atomids)
            idxs = idxs | (surface_aismg == atomids(j));
        end
        idxs = find(idxs);
        resType = protein.resType(i,:);
        resSASA = compute_SASA(surface_rdsmg(idxs));
        disp(sprintf('residue %2d: type(%s), #points(%3d), SASA(%8.4f A^2)', i, resType, length(idxs), resSASA));
    end
    
    disp(sprintf('\n'))
    disp('solvent accessible surface area of protein');
    proteinSASA = compute_SASA(surface_rdsmg);
    disp(sprintf('#points(%3d), SASA(%8.4f A^2)', length(surface_rdsmg), proteinSASA));
    surfpts = surface_ptsmg;

    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    disp(sprintf('\n'))
    disp('write 2GB1-surface.pdb');
    atom=pdb.Model.Atom;
    pdb.Model.Atom = [pdb.Model.Atom, atom(ones(1,size(surfpts,1)))];
    for i=1:size(surfpts,1)
        idx = protein_atoms_num + i;
        pdb.Model.Atom(idx).AtomSerNo = idx;
        pdb.Model.Atom(idx).AtomName  = 'H';
        pdb.Model.Atom(idx).altLoc    = '';
        pdb.Model.Atom(idx).resName   = '   ';
        pdb.Model.Atom(idx).chainID   = 'A';
        pdb.Model.Atom(idx).resSeq    = pdb.Model.Terminal.resSeq + 1;
        pdb.Model.Atom(idx).iCode     = '';
        pdb.Model.Atom(idx).X = surfpts(i,1);
        pdb.Model.Atom(idx).Y = surfpts(i,2);
        pdb.Model.Atom(idx).Z = surfpts(i,3);
        pdb.Model.Atom(idx).occupancy = 1;
        pdb.Model.Atom(idx).tempFactor = 0;
        pdb.Model.Atom(idx).segID      = '    ';
        pdb.Model.Atom(idx).element    = 'H';
        pdb.Model.Atom(idx).charge     = '  ';
        pdb.Model.Atom(idx).AtomNameStruct.chemSymbol = 'H';
        pdb.Model.Atom(idx).AtomNameStruct.remoteInd = '';
        pdb.Model.Atom(idx).AtomNameStruct.branch = '';
    end
    pdb.Model.Terminal.SerialNo = protein_atoms_num + size(surfpts,1) + 1;
    pdb.Model.Terminal.resName = '   ';
    pdb.Model.Terminal.chainID = 'A';
    pdb.Model.Terminal.resSeq  = pdb.Model.Terminal.resSeq + 1;
    pdb.Model.Terminal.iCode   = '';
    pdbwrite('2GB1-surface.pdb', pdb);
end

function SASA = compute_SASA(rdsmg)
    SASA = 0;
    while(~isempty(rdsmg))
        rd = rdsmg(1);
        idx = (rdsmg == rd);
        rdcnt = length(find(idx));
        area = double(rdcnt * 4 * pi * rd*rd / 500);
        SASA = SASA + area;
        rdsmg = rdsmg(~idx);
    end
end

function [pts, acs, cutoff] = build_spherepoints()
    % build 500 points at the surface of a sphere with center (0,0,0) and
    % radius 1.
    numpts = 500;
    mkdir('cache');
    if(~exist('cache/pts.mat'))
        numptstot = numpts * 10;
        pts = zeros(numptstot,3);
        cnt = 0;
        while(cnt < numptstot)
            % http://mathworld.wolfram.com/SpherePointPicking.html
            x = rand(4,1);
            x = 2*x - [1;1;1;1];
            x0 = x(1);
            x1 = x(2);
            x2 = x(3);
            x3 = x(4);
            xx = x' * x;
            if(xx >= 1) continue; end
            cnt = cnt + 1;
            pts(cnt,1) = 2*(x1*x3 + x0*x2);
            pts(cnt,2) = 2*(x2*x3 - x0*x1);
            pts(cnt,3) = x0*x0 + x3*x3 - x1*x1 - x2*x2;
            pts(cnt,:) = pts(cnt,:) / xx;
        end
        acs = pdist(pts);
        acs = squareform(acs);
        while(size(pts,1) > numpts)
            min_acs = min(squareform(acs));
            [r,c] = find(acs==min_acs,1);
            assert(r ~= c);
            selpts = setdiff(1:size(pts,1), [r,c]);
            rmin = min(acs(r,selpts));
            cmin = min(acs(c,selpts));
            if(rmin > cmin)
                selpts = setdiff(1:size(pts,1), c);
            else
                selpts = setdiff(1:size(pts,1), r);
            end
            pts = pts(selpts,:);
            acs = acs(selpts,selpts);
            %[size(pts,1), min_acs]
        end
        save('cache/pts.mat','pts');
    end
    load('cache/pts.mat','pts');

    acs = pdist(pts);
    acs_min = min(acs);
    cutoff = acs_min*1.5;
    acs = acs < cutoff;
    acs = squareform(acs);
    acs = triu(acs);
    acs = sparse(acs);
    sum(sum(acs))
end

function [ptsmg, acsmg, rdsmg, aismg] = build_pts_acs(atomids, protein, pts, acs, rds, ais, cutoff)
    function [pts, acs, tps, ais] = remove_invalid(pts, acs, tps, ais, atomcenter, radii, idx)
        idx = pdist2(pts, atomcenter);
        idx = find(idx < radii);
        idx = setdiff(1:size(pts), idx);
        pts = pts(idx,:);
        acs = acs(idx,idx);
        tps = tps(idx,:);
        ais = ais(idx,:);
    end
    ptsmg = pts{atomids(1)};
    acsmg = acs{atomids(1)};
    rdsmg = rds{atomids(1)};
    aismg = ais{atomids(1)};
    for i=2:length(atomids)
        atomid = atomids(i);
        % remove pts inside atomid
        atomcenter = protein.coord(atomid,:);
        radii = protein.atomRadii(atomid);
        [ptsmg, acsmg, rdsmg, aismg] = remove_invalid(ptsmg, acsmg, rdsmg, aismg, atomcenter, radii);
        % remove pts inside mergeds
        ptstg = pts{atomid};
        acstg = acs{atomid};
        rdstg = rds{atomid};
        aistg = ais{atomid};
        for j=1:i-1
            atomid_ = atomids(j);
            atomcenter = protein.coord(atomid_,:);
            radii = protein.atomRadii(atomid_);
            [ptstg, acstg, rdstg, aistg] = remove_invalid(ptstg, acstg, rdstg, aistg, atomcenter, radii);
        end
        % add edges to acs
        idx = pdist2(ptsmg, ptstg);
        idx = idx < cutoff;
        ptsmg = [ptsmg; ptstg];
        acsmg = [acsmg, idx; idx', acstg];
        rdsmg = [rdsmg; rdstg];
        aismg = [aismg; aistg];
        %disp(sprintf('marge atom %d',i));
    end
end

function grps = build_indexgroup(acsmg)
    [S,C]=graphconncomp(acsmg,'Weak',true);
    % count size of groups
    cnt = zeros(S,2);
    for i=1:S
        cnt(i,1) = sum(C == i);
        cnt(i,2) = i;
    end
    cnt = sortrows(cnt);
    grps = cell(S,1);
    for i=1:S
        idx = S + 1 - i;
        grpcnt = cnt(idx,1);
        grpidx = cnt(idx,2);
        grps{i} = find(C == grpidx);
    end
end

