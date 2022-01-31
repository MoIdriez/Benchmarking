function scatterRange(M,r,xlbl,ylbl,xlimit,ylimit)

figure;

for i = 1:length(r)
    N = M(M(:,3) > r(i), :);
    scatterRangeSubPlot(2,2,i,N(:,1), N(:,2),N(:,3),xlbl,ylbl,xlimit,ylimit)
end

