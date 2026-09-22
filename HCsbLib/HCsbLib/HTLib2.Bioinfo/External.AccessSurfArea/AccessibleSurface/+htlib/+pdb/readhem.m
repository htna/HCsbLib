function atoms = readhem(pdb)
    if(isa(pdb,'char'))
        pdb = pdbread(pdb);
    end
    pdb = pdb.Model.HeterogenAtom;
    atoms.AtomName = {pdb.AtomName}'; % N, CA, C, O, CA, CB, ...
    atoms.altLoc   = {pdb.altLoc}'; % '', 'A', 'B', 'C'
    atoms.resName  = {pdb.resName}'; % VAL, LEU, ...
    atoms.resSeq   = [pdb.resSeq]'; % 1, 2, 3, ...
    atoms.coord    = [[pdb.X]', [pdb.Y]', [pdb.Z]'];
    atoms.element  = [pdb.element]'; % N, C, O, ...
    
    idxsKeep = htlib.cell.find(atoms.resName,'HEM');

    atoms.AtomName = atoms.AtomName(idxsKeep);
    atoms.altLoc   = atoms.altLoc  (idxsKeep);
    atoms.resName  = atoms.resName (idxsKeep);
    atoms.resSeq   = atoms.resSeq  (idxsKeep);
    atoms.coord    = atoms.coord   (idxsKeep,:);
    atoms.element  = atoms.element (idxsKeep);
end
