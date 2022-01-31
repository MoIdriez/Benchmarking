%Map,Success,IsStuck,Time,Steps,MaxIterations,ObstacleRange,ObstacleConstant,AttractiveConstant
M = readmatrix("Data\PotentialField-Extensive-AllSuccessScatter.txt");


scatterRange(M(:,[1 2 4]), [0 20 30 40] ...
    ,'ObstacleRange', 'ObstacleConstant' ...
    ,[0 10], [0 20]);


scatterRange(M(:,[1 3 4]), [0 20 30 40] ...
    ,'ObstacleRange', 'AttractiveConstant' ...
    ,[0 10], [0 20]);

scatterRange(M(:,[2 3 4]), [0 20 30 40] ...
    ,'ObstacleConstant', 'AttractiveConstant' ...
    ,[0 10], [0 20]);


% figure;
% scatter3(M(:,1),M(:,2),M(:,3),150,M(:,4) ...
%     ,'filled');
% colorbar;
% xlabel('ObstacleRange');
% ylabel('ObstacleConstant');
% zlabel('AttractiveConstant');
% xlim([0 40]);
% ylim([0 20]);
% zlim([0 20]);
% 
% figure;
% scatter(M(:,1),M(:,2),[],M(:,4) ...
%     ,'filled');
% colorbar;
% xlabel('ObstacleRange');
% ylabel('ObstacleConstant');
% xlim([0 40]);
% ylim([0 20]);
% 
% 
% figure;
% scatter(M(:,1),M(:,3),[],M(:,4) ...
%     ,'filled');
% colorbar;
% xlabel('ObstacleRange');
% ylabel('AttractiveConstant');
% xlim([0 40]);
% ylim([0 20]);
% 
% 
% figure;
% scatter(M(:,2),M(:,3),[],M(:,4) ...
%     ,'filled');
% colorbar;
% xlabel('ObstacleConstant');
% ylabel('AttractiveConstant');
% xlim([0 20]);
% ylim([0 20]);
% 
% % figure;
% % W = M(M(:,4) > 30, :);
% % s = scatter3(W(:,1),W(:,2),W(:,3),50,W(:,4));
% % colorbar;
% % xlabel('ObstacleRange');
% % ylabel('ObstacleConstant');
% % zlabel('AttractiveConstant');
% % xlim([0 40]);
% % ylim([0 20]);
% % zlim([0 20]);
% 
