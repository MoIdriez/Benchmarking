function scatterRangeSubPlot(m,n,p,x,y,c,xlbl,ylbl,xlimit,ylimit,csx)
subplot(m,n,p);
scatter(x,y,[],c ...
    ,'filled');
colorbar;
caxis(csx);
xlabel(xlbl);
ylabel(ylbl);
xlim(xlimit);
ylim(ylimit);
end

