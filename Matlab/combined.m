%ObstacleRange,ObstacleConstant,AttractiveConstant -- Counter,Constant,StrengthIncrease,Range
p = readmatrix("Data\PotentialField-Extensive-AllSuccessScatter.txt");
pf = readmatrix("Data\PheromonePotentialField-Extensive-AllScatter.txt");

% figure for all of the potential fields combined
figure;

%==============================================
N = p(p(:,4) > 0, :);
scatterRangeSubPlot(3,4,1,N(:,1),N(:,2),N(:,4), 'ObstacleRange', 'ObstacleConstant', [0 40], [0 20], [0 300]);
title('>0');

N = p(p(:,4) > 30, :);
scatterRangeSubPlot(3,4,2,N(:,1),N(:,2),N(:,4), 'ObstacleRange', 'ObstacleConstant', [0 40], [0 20], [0 300]);
title('>30');

N = p(p(:,4) > 60, :);
scatterRangeSubPlot(3,4,3,N(:,1),N(:,2),N(:,4), 'ObstacleRange', 'ObstacleConstant', [0 40], [0 20], [0 300]);
title('>60');

N = p(p(:,4) > 90, :);
scatterRangeSubPlot(3,4,4,N(:,1),N(:,2),N(:,4), 'ObstacleRange', 'ObstacleConstant', [0 40], [0 20], [0 300]);
title('>90');
%==============================================
N = p(p(:,4) > 0, :);
scatterRangeSubPlot(3,4,5,N(:,1),N(:,3),N(:,4), 'ObstacleRange', 'AttractiveConstant', [0 40], [0 20], [0 300]);
title('>0');

N = p(p(:,4) > 30, :);
scatterRangeSubPlot(3,4,6,N(:,1),N(:,3),N(:,4), 'ObstacleRange', 'AttractiveConstant', [0 40], [0 20], [0 300]);
title('>30');

N = p(p(:,4) > 60, :);
scatterRangeSubPlot(3,4,7,N(:,1),N(:,3),N(:,4), 'ObstacleRange', 'AttractiveConstant', [0 40], [0 20], [0 300]);
title('>60');

N = p(p(:,4) > 90, :);
scatterRangeSubPlot(3,4,8,N(:,1),N(:,3),N(:,4), 'ObstacleRange', 'AttractiveConstant', [0 40], [0 20], [0 300]);
title('>90');
%==============================================
N = p(p(:,4) > 0, :);
scatterRangeSubPlot(3,4,9,N(:,2),N(:,3),N(:,4), 'ObstacleConstant', 'AttractiveConstant', [0 20], [0 20], [0 300]);
title('>0');

N = p(p(:,4) > 30, :);
scatterRangeSubPlot(3,4,10,N(:,2),N(:,3),N(:,4), 'ObstacleConstant', 'AttractiveConstant', [0 20], [0 20], [0 300]);
title('>30');

N = p(p(:,4) > 60, :);
scatterRangeSubPlot(3,4,11,N(:,2),N(:,3),N(:,4), 'ObstacleConstant', 'AttractiveConstant', [0 20], [0 20], [0 300]);
title('>60');

N = p(p(:,4) > 90, :);
scatterRangeSubPlot(3,4,12,N(:,2),N(:,3),N(:,4), 'ObstacleConstant', 'AttractiveConstant', [0 20], [0 20], [0 300]);
title('>90');
%==============================================

% figure for all of the potential fields combined
figure;

%==============================================
N = p(p(:,4) > 0, :);
scatterRangeSubPlot(3,4,1,N(:,1),N(:,2),N(:,4), 'ObstacleRange', 'ObstacleConstant', [0 10], [0 20], [0 50]);
title('>0');

N = p(p(:,4) > 20, :);
scatterRangeSubPlot(3,4,2,N(:,1),N(:,2),N(:,4), 'ObstacleRange', 'ObstacleConstant', [0 10], [0 20], [0 50]);
title('>20');

N = p(p(:,4) > 30, :);
scatterRangeSubPlot(3,4,3,N(:,1),N(:,2),N(:,4), 'ObstacleRange', 'ObstacleConstant', [0 10], [0 20], [0 50]);
title('>30');

N = p(p(:,4) > 40, :);
scatterRangeSubPlot(3,4,4,N(:,1),N(:,2),N(:,4), 'ObstacleRange', 'ObstacleConstant', [0 10], [0 20], [0 50]);
title('>40');
%==============================================
N = p(p(:,4) > 0, :);
scatterRangeSubPlot(3,4,5,N(:,1),N(:,3),N(:,4), 'ObstacleRange', 'AttractiveConstant', [0 10], [0 20], [0 50]);
title('>0');

N = p(p(:,4) > 20, :);
scatterRangeSubPlot(3,4,6,N(:,1),N(:,3),N(:,4), 'ObstacleRange', 'AttractiveConstant', [0 10], [0 20], [0 50]);
title('>20');

N = p(p(:,4) > 30, :);
scatterRangeSubPlot(3,4,7,N(:,1),N(:,3),N(:,4), 'ObstacleRange', 'AttractiveConstant', [0 10], [0 20], [0 50]);
title('>30');

N = p(p(:,4) > 40, :);
scatterRangeSubPlot(3,4,8,N(:,1),N(:,3),N(:,4), 'ObstacleRange', 'AttractiveConstant', [0 10], [0 20], [0 50]);
title('>40');
%==============================================
N = p(p(:,4) > 0, :);
scatterRangeSubPlot(3,4,9,N(:,2),N(:,3),N(:,4), 'ObstacleConstant', 'AttractiveConstant', [0 20], [0 20], [0 50]);
title('>0');

N = p(p(:,4) > 20, :);
scatterRangeSubPlot(3,4,10,N(:,2),N(:,3),N(:,4), 'ObstacleConstant', 'AttractiveConstant', [0 20], [0 20], [0 50]);
title('>20');

N = p(p(:,4) > 30, :);
scatterRangeSubPlot(3,4,11,N(:,2),N(:,3),N(:,4), 'ObstacleConstant', 'AttractiveConstant', [0 20], [0 20], [0 50]);
title('>30');

N = p(p(:,4) > 40, :);
scatterRangeSubPlot(3,4,12,N(:,2),N(:,3),N(:,4), 'ObstacleConstant', 'AttractiveConstant', [0 20], [0 20], [0 50]);
title('>40');
%==============================================

% figure for all of the pheromone potential fields combined
figure;
%==============================================
N = pf(pf(:,4) > 0, :);
scatterRangeSubPlot(3,4,1,N(:,1),N(:,2),N(:,4), 'ObstacleRange', 'ObstacleConstant', [0 10], [0 10], [0 30]);
title('>0');

N = pf(pf(:,4) > 10, :);
scatterRangeSubPlot(3,4,2,N(:,1),N(:,2),N(:,4), 'ObstacleRange', 'ObstacleConstant', [0 10], [0 10], [0 30]);
title('>10');

N = pf(pf(:,4) > 20, :);
scatterRangeSubPlot(3,4,3,N(:,1),N(:,2),N(:,4), 'ObstacleRange', 'ObstacleConstant', [0 10], [0 10], [0 30]);
title('>20');

N = pf(pf(:,4) > 25, :);
scatterRangeSubPlot(3,4,4,N(:,1),N(:,2),N(:,4), 'ObstacleRange', 'ObstacleConstant', [0 10], [0 10], [0 30]);
title('>25');
%==============================================
N = pf(pf(:,4) > 0, :);
scatterRangeSubPlot(3,4,5,N(:,1),N(:,3),N(:,4), 'ObstacleRange', 'AttractiveConstant', [0 10], [0 10], [0 30]);
title('>0');

N = pf(pf(:,4) > 10, :);
scatterRangeSubPlot(3,4,6,N(:,1),N(:,3),N(:,4), 'ObstacleRange', 'AttractiveConstant', [0 10], [0 10], [0 30]);
title('>10');

N = pf(pf(:,4) > 20, :);
scatterRangeSubPlot(3,4,7,N(:,1),N(:,3),N(:,4), 'ObstacleRange', 'AttractiveConstant', [0 10], [0 10], [0 30]);
title('>20');

N = pf(pf(:,4) > 25, :);
scatterRangeSubPlot(3,4,8,N(:,1),N(:,3),N(:,4), 'ObstacleRange', 'AttractiveConstant', [0 10], [0 10], [0 30]);
title('>25');
%==============================================
N = pf(pf(:,4) > 0, :);
scatterRangeSubPlot(3,4,9,N(:,2),N(:,3),N(:,4), 'ObstacleConstant', 'AttractiveConstant', [0 10], [0 10], [0 30]);
title('>0');

N = pf(pf(:,4) > 10, :);
scatterRangeSubPlot(3,4,10,N(:,2),N(:,3),N(:,4), 'ObstacleConstant', 'AttractiveConstant', [0 10], [0 10], [0 30]);
title('>10');

N = pf(pf(:,4) > 20, :);
scatterRangeSubPlot(3,4,11,N(:,2),N(:,3),N(:,4), 'ObstacleConstant', 'AttractiveConstant', [0 10], [0 10], [0 30]);
title('>20');

N = pf(pf(:,4) > 25, :);
scatterRangeSubPlot(3,4,12,N(:,2),N(:,3),N(:,4), 'ObstacleConstant', 'AttractiveConstant', [0 10], [0 10], [0 30]);
title('>25');
%==============================================

% figure for all of the pheromone potential fields combined
figure;
%==============================================
N = pf(pf(:,4) > 0, :);
scatterRangeSubPlot(3,4,1,N(:,7),N(:,5),N(:,4), 'PheromoneRange', 'PheromoneConstant', [0 10], [0 10], [0 30]);
title('>0');

N = pf(pf(:,4) > 10, :);
scatterRangeSubPlot(3,4,2,N(:,7),N(:,5),N(:,4), 'PheromoneRange', 'PheromoneConstant', [0 10], [0 10], [0 30]);
title('>10');

N = pf(pf(:,4) > 20, :);
scatterRangeSubPlot(3,4,3,N(:,7),N(:,5),N(:,4), 'PheromoneRange', 'PheromoneConstant', [0 10], [0 10], [0 30]);
title('>20');

N = pf(pf(:,4) > 25, :);
scatterRangeSubPlot(3,4,4,N(:,7),N(:,5),N(:,4), 'PheromoneRange', 'PheromoneConstant', [0 10], [0 10], [0 30]);
title('>25');
%==============================================
N = pf(pf(:,4) > 0, :);
scatterRangeSubPlot(3,4,5,N(:,7),N(:,6),N(:,4), 'PheromoneRange', 'StrengthIncrease', [0 10], [0 10], [0 30]);
title('>0');

N = pf(pf(:,4) > 10, :);
scatterRangeSubPlot(3,4,6,N(:,7),N(:,6),N(:,4), 'PheromoneRange', 'StrengthIncrease', [0 10], [0 10], [0 30]);
title('>10');

N = pf(pf(:,4) > 20, :);
scatterRangeSubPlot(3,4,7,N(:,7),N(:,6),N(:,4), 'PheromoneRange', 'StrengthIncrease', [0 10], [0 10], [0 30]);
title('>20');

N = pf(pf(:,4) > 25, :);
scatterRangeSubPlot(3,4,8,N(:,7),N(:,6),N(:,4), 'PheromoneRange', 'StrengthIncrease', [0 10], [0 10], [0 30]);
title('>25');
%==============================================
N = pf(pf(:,4) > 0, :);
scatterRangeSubPlot(3,4,9,N(:,5),N(:,6),N(:,4), 'PheromoneConstant', 'StrengthIncrease', [0 10], [0 10], [0 30]);
title('>0');

N = pf(pf(:,4) > 10, :);
scatterRangeSubPlot(3,4,10,N(:,5),N(:,6),N(:,4), 'PheromoneConstant', 'StrengthIncrease', [0 10], [0 10], [0 30]);
title('>10');

N = pf(pf(:,4) > 20, :);
scatterRangeSubPlot(3,4,11,N(:,5),N(:,6),N(:,4), 'PheromoneConstant', 'StrengthIncrease', [0 10], [0 10], [0 30]);
title('>20');

N = pf(pf(:,4) > 25, :);
scatterRangeSubPlot(3,4,12,N(:,5),N(:,6),N(:,4), 'PheromoneConstant', 'StrengthIncrease', [0 10], [0 10], [0 30]);
title('>25');
%==============================================