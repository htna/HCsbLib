function mat = lists2mat(lists)
% mat = lists2mat(lists)
%
% lists = {[1,2,3], [2,4], [1,3,5,6]}
% mat = [1,2,3,0;
%        2,4,0,0;
%        1,3,5,6];
    maxlen = 0;
    for i=1:length(lists)
        maxlen = max(maxlen, length(lists{i}));
    end

    mat = zeros(length(lists), maxlen);
    for i=1:length(lists)
        mat(i,1:length(lists{i})) = lists{i};
    end
end
