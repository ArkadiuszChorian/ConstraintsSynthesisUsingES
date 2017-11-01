using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Accord.Math;
using CSUES.Engine.Models.Constraints;
using Gurobi;

namespace CSUES.Engine.Measurement
{
    public class MeanAngleCalculator : IAngleCalculator
    {
        public double Calculate(IList<Constraint> synthesizedConstraints, IList<Constraint> referenceConstraints)
        {
            // Assume benchmark restrictions indexes are bound to rows
            var matrix = new double[referenceConstraints.Count, synthesizedConstraints.Count];

            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                for (var j = 0; j < matrix.GetLength(1); j++)
                    matrix[i, j] = AngleBetweenVectors(referenceConstraints[i].GetAllCoefficients(), synthesizedConstraints[j].GetAllCoefficients());
            }

            // Assignment problem
            var env = new GRBEnv("angle.log");
            var model = new GRBModel(env)
            {
                ModelSense = GRB.MINIMIZE,
                ModelName = "Mean Angle Assignment Problem"
            };

            // Binary variable for each matrix cell
            var grbVars = new GRBVar[matrix.GetLength(0), matrix.GetLength(1)];

            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                for (var j = 0; j < matrix.GetLength(1); j++)
                {
                    grbVars[i, j] = model.AddVar(0.0, 1.0, 0.0, GRB.BINARY, $"b{i}.{j}");
                }
            }

            var contraintCounter = 0;
            var objectiveExpr = new GRBLinExpr();

            // First add contraints for each row
            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                var rowExpr = new GRBLinExpr();
                for (var j = 0; j < matrix.GetLength(1); j++)
                {
                    rowExpr.AddTerm(1.0, grbVars[i, j]);
                    // Creating objective equation ONLY ONCE !
                    objectiveExpr.AddTerm(matrix[i, j], grbVars[i, j]);
                }
                model.AddConstr(rowExpr, GRB.GREATER_EQUAL, 1, $"c{contraintCounter++}");
            }

            // Transpose matrix so we can perform the same operation
            matrix = matrix.Transpose();

            // Add contstaints for originally columns
            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                var rowExpr = new GRBLinExpr();
                for (var j = 0; j < matrix.GetLength(1); j++)
                {
                    // Inverted indexes i and j
                    rowExpr.AddTerm(1.0, grbVars[j, i]);
                }
                model.AddConstr(rowExpr, GRB.GREATER_EQUAL, 1, $"c{contraintCounter++}");
            }

            model.SetObjective(objectiveExpr);

            model.Optimize();

            // Back to orignal shape
            matrix = matrix.Transpose();

            // Gets indicators for each bianry variable for solution
            var binarySolutions = new double[matrix.GetLength(0), matrix.GetLength(1)];

            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                for (var j = 0; j < matrix.GetLength(1); j++)
                {
                    binarySolutions[i, j] = grbVars[i, j].X;
                }
            }

            // Multiplies binary indicators with coeficients
            var sum = Matrix.ElementwiseMultiply(matrix, binarySolutions).Sum();

            model.Write("angle_out.lp");

            model.Dispose();
            env.Dispose();

            // Return mean angle
            return sum / (matrix.GetLength(0) > matrix.GetLength(1)
                ? matrix.GetLength(0)
                : matrix.GetLength(1));
        }

        private static double AngleBetweenVectors(double[] first, double[] second)
        {
            if (first.Length != second.Length)
                throw new InvalidEnumArgumentException("Vectors dimensions not equal");

            var acc = first.Select((t, i) => t * second[i]).Sum();
            var denom = Math.Sqrt(first.Sum(x => Math.Pow(x, 2)))
                    * Math.Sqrt(second.Sum(x => Math.Pow(x, 2)));
            return Math.Acos(acc / denom);
        }       
    }
}