function idxs = find(data, query)
%
% htlib.cell.find({'a','ab','abc'}, 'ab') => 2
% htlib.cell.find({'a','ab','abc'}, '')   => Empty matrix: 0-by-1
%
    idxs = zeros(length(data),1);
    for i=1:length(data)
        if(isequal(data{i}, query))
            idxs(i) = 1;
        end
    end
    idxs = find(idxs);
end
