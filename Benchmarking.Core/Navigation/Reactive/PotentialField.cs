﻿using Benchmarking.Core.Map;
using Benchmarking.Core.Navigation.Helper;
using Benchmarking.Core.Navigation.Models;

namespace Benchmarking.Core.Navigation.Reactive
{
    public class PotentialField : NavigationalMethod
    {
        private readonly PfSettings _settings;

        public PotentialField(int[,] fullMap, Robot robot, Point goal, int maxIterations, PfSettings settings) : base(fullMap, robot, goal, maxIterations)
        {
            _settings = settings;
        }

        protected override void Loop()
        {
            var force = NavExt.CalculatePotential(ExploredMap, _settings, Robot.Location, Goal);
            var nextStep = Robot.Location + force.GridPoint();
            Robot.Step(Map.CanStepTo(Robot.Location, nextStep) ? nextStep : Robot.Location);
        }

        protected override string AdditionalMetrics()
        {
            return $",{_settings}";
        }
    }
}