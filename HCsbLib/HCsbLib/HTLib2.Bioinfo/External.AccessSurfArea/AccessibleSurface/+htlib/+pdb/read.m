function atoms = read(pdb, types)
% htlib.pdb.read('1a6g.pdb',{'atom','hem'})
% htlib.pdb.read('1a6g.pdb',{'atom'})
% htlib.pdb.read(pdbread('1a6g.pdb'),{'atom','hem'})
    if(isa(pdb,'char'))
        pdb = pdbread(pdb);
    end
    
    atoms.AtomName = {}; % N, CA, C, O, CA, CB, ...
    atoms.altLoc   = {}; % '', 'A', 'B', 'C'
    atoms.resName  = {}; % VAL, LEU, ...
    atoms.resSeq   = []; % 1, 2, 3, ...
    atoms.coord    = [];
    atoms.element  = []; % N, C, O, ...
    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    atoms.atomType = []; % ['N  ';'CA ';'C  '; ...]
    atoms.resType  = []; % ['VAL';'LEU';'HEM'; ...]
    
    function meargewith(latoms)
        atoms.AtomName = [atoms.AtomName ; latoms.AtomName];
        atoms.altLoc   = [atoms.altLoc   ; latoms.altLoc  ];
        atoms.resName  = [atoms.resName  ; latoms.resName ];
        atoms.resSeq   = [atoms.resSeq   ; latoms.resSeq  ];
        atoms.coord    = [atoms.coord    ; latoms.coord   ];
        atoms.element  = [atoms.element  ; latoms.element ];
        %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        for i=1:length(latoms.AtomName)
            latoms_atomType = [latoms.AtomName{i}, '   '];
            atoms.atomType = [atoms.atomType ; latoms_atomType(1:3)];
        end
        atoms.resType  = [atoms.resType  ; cell2mat(latoms.resName)];
    end

    if(~isempty(htlib.cell.find(types,'atom')))
        latoms = htlib.pdb.readatom(pdb);
        meargewith(latoms);
    end
    if(~isempty(htlib.cell.find(types,'hem')))
        latoms = htlib.pdb.readhem(pdb);
        meargewith(latoms);
    end
end
