namespace MachineLearning.Classification.Utils
{
    using System.Collections.Generic;

    using Microsoft.SolverFoundation.Services;
    using Microsoft.SolverFoundation.Solvers;

    internal class QuasiNewtonOptimizer
    {
        private readonly CompactQuasiNewtonSolverParams solverParams = new CompactQuasiNewtonSolverParams();
        private DiffFunc func;
        private double[] curGradient;
        private double[] curVariableVals;
        private double curVal;
        private int[] variables;

        /// <summary>
        /// A delegate for functions with gradients.
        /// </summary>
        /// <param name="value">The point at which to evaluate the function</param><param name="grad">The gradient</param>
        /// <returns>
        /// The value of the function
        /// </returns>
        public delegate double DiffFunc(IList<double> value, IList<double> grad);


        /// <summary>
        /// Minimize the function
        /// </summary>
        /// <param name="diffFunc">Function to minimize</param><param name="startingPoint">Starting point</param>
        /// <returns>
        /// The minimum
        /// </returns>
        public double[] Minimize(DiffFunc diffFunc, double[] startingPoint)
        {
            var solver = new CompactQuasiNewtonSolver();

            var length = startingPoint.Length;
            this.variables = new int[length];
            for (var index = 0; index < length; ++index)
            {
                solver.AddVariable(null, out this.variables[index]);
                solver.SetValue(this.variables[index], startingPoint[index]);
            }

            int vid;
            solver.AddRow(null, out vid);
            solver.AddGoal(vid, 0, true);
            solver.FunctionEvaluator = this.FunctionEvaluator;
            solver.GradientEvaluator = this.GradientEvaluator;

            this.func = diffFunc;
            this.curGradient = new double[length];
            this.curVariableVals = new double[length];

            var nonlinearSolution = solver.Solve(this.solverParams);

            var numArray = new double[length];
            for (var index = 0; index < length; ++index)
            {
                numArray[index] = nonlinearSolution.GetValue(this.variables[index]);
            }

            return numArray;
        }

        private double FunctionEvaluator(INonlinearModel model, int rowVid, ValuesByIndex values, bool newValues)
        {
            if (newValues)
            {
                this.Compute(values);
            }

            return this.curVal;
        }

        private void Compute(ValuesByIndex values)
        {
            for (var index = 0; index < this.curVariableVals.Length; ++index)
            {
                this.curVariableVals[index] = values[this.variables[index]];
            }

            this.curVal = this.func(this.curVariableVals, this.curGradient);
        }

        private void GradientEvaluator(INonlinearModel model, int rowVid, ValuesByIndex values, bool newValues, ValuesByIndex gradient)
        {
            if (newValues)
            {
                this.Compute(values);
            }

            for (var index = 0; index < this.curGradient.Length; ++index)
            {
                gradient[this.variables[index]] = this.curGradient[index];
            }
        }
    }
}