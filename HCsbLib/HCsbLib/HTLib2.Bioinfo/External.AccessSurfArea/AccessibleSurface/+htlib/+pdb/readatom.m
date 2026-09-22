function atoms = readatom(pdb)
    if(isa(pdb,'char'))
        pdb = pdbread(pdb);
    end
    pdb = pdb.Model.Atom;
    atoms.AtomName = {pdb.AtomName}'; % N, CA, C, O, CA, CB, ...
    atoms.altLoc   = {pdb.altLoc}'; % '', 'A', 'B', 'C'
    atoms.resName  = {pdb.resName}'; % VAL, LEU, ...
    atoms.resSeq   = [pdb.resSeq]'; % 1, 2, 3, ...
    atoms.coord    = [[pdb.X]', [pdb.Y]', [pdb.Z]'];
    atoms.element  = [pdb.element]'; % N, C, O, ...
    
    idxsAltB = htlib.cell.find(atoms.altLoc, 'B');
    idxsAltC = htlib.cell.find(atoms.altLoc, 'C');
    idxsKeep = 1:length(pdb);
    idxsKeep = setdiff(idxsKeep, idxsAltB);
    idxsKeep = setdiff(idxsKeep, idxsAltC);

    atoms.AtomName = atoms.AtomName(idxsKeep);
    atoms.altLoc   = atoms.altLoc  (idxsKeep);
    atoms.resName  = atoms.resName (idxsKeep);
    atoms.resSeq   = atoms.resSeq  (idxsKeep);
    atoms.coord    = atoms.coord   (idxsKeep,:);
    atoms.element  = atoms.element (idxsKeep);
end
