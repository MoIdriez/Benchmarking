using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Core.Navigation.Helper;
using Benchmarking.Core.Navigation.Models;

namespace Benchmarking.Core.Navigation.Reactive
{
    public class PheromonePotentialField : NavigationalMethod
    {
        private readonly PotentialFieldSettings _settings;
        private readonly PheromoneSettings _pheromoneSettings;
        private readonly List<Pheromone> _pheromones = new();

        public PheromonePotentialField(int[,] fullMap, Robot robot, Point goal, int maxIterations, PotentialFieldSettings settings, PheromoneSettings pheromoneSettings) : base(fullMap, robot, goal, maxIterations)
        {
            _settings = settings;
            _pheromoneSettings = pheromoneSettings;
            IsStuckVerifier = 20;
        }

        protected override void Loop()
        {
            var force = NavExt.CalculatePotential(ExploredMap, _settings, Robot.Location, Goal) + CalculatePheromones();
            var nextStep = Robot.Location + force.GridPoint();

            var pheromone = Map.CanStepTo(Robot.Location, nextStep) ? Robot.Location : nextStep;
            if (_pheromones.Any(p => p.Location.Equals(pheromone)))
            {
                _pheromones.First(p => p.Location.Equals(pheromone)).Strength += _pheromoneSettings.StrengthIncrease;
            }
            else
            {
                _pheromones.Add(new Pheromone(pheromone, _pheromoneSettings.StrengthIncrease));
            }

            RobotStep(Map.CanStepTo(Robot.Location, nextStep) ? nextStep : Robot.Location);
        }

        private Vector CalculatePheromones()
        {
            var pheromoneRepulsive = new Vector(0, 0);
            foreach (var pheromone in _pheromones)
            {
                if (pheromone.Location.DistanceTo(Robot.Location) > _pheromoneSettings.Range) continue;
                var c = _pheromoneSettings.Constant * pheromone.Strength;
                pheromoneRepulsive += Robot.Location.Repulsive(pheromone.Location, c, 1);
            }

            return pheromoneRepulsive;
        }

        protected override string AdditionalMetrics()
        {
            return $",{_settings},{_pheromoneSettings}";
        }

        private class Pheromone
        {
            public Pheromone(Point location, double strength)
            {
                Location = location;
                Strength = strength;
            }

            public Point Location { get; }
            public double Strength { get; set; }
        }
    }
}
