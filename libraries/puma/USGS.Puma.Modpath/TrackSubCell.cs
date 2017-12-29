using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoAPI.Geometries;
using USGS.Puma.Core;
using USGS.Puma.Modpath;
using USGS.Puma.NTS.Geometries;

namespace USGS.Puma.Modpath
{
    public enum TrackingStatus
    {
        Undefined = 0,
        ExitAtCellFace = 1,
        MaximumTimeReached = 2,
        NoExitPossible = 3
    }

    public class TrackSubCell
    {
        #region Fields
        #endregion

        #region Constructors
        public TrackSubCell()
        { }
        #endregion

        #region Public Members

        public TrackSubCellResult ExecuteTracking(bool steadyState, SubCellData cellData, ParticleLocation initialLocation, double maximumTime)
        {
            if (initialLocation == null)
                throw new ArgumentNullException();

            return ExecuteTracking(steadyState, cellData, initialLocation.CellNumber, initialLocation.LocalX, initialLocation.LocalY, initialLocation.LocalZ, initialLocation.TrackingTime, maximumTime);
        }

        public TrackSubCellResult ExecuteTracking(bool steadyState, SubCellData cellData, int cellNumber, double initialX, double initialY, double initialZ, double initialTime, double maximumTime)
        {
            TrackSubCellResult result = new TrackSubCellResult();
            result.InitialLocation = new ParticleLocation(cellNumber, initialX, initialY, initialZ, initialTime);
            result.MaximumTime = maximumTime;
            
            if (steadyState)
            {
                if (!cellData.HasExitFace())
                {
                    result.Status = TrackingStatus.NoExitPossible;
                    return result;
                }
            }
            
            double vx1 = cellData.VX1;
            double vx2 = cellData.VX2;
            double vy1 = cellData.VY1;
            double vy2 = cellData.VY2;
            double vz1 = cellData.VZ1;
            double vz2 = cellData.VZ2;

            // Declare directional velocity components
            double vx;
            double dvxdx;
            double dtx;
            double vy;
            double dvydy;
            double dty;
            double vz;
            double dvzdz;
            double dtz;
            double dt = 0;

            // Compute the time of travel to each possible exit face
            int statusVX = CalculateDT(vx1, vx2, cellData.DX, initialX, out vx, out dvxdx, out dtx);
            int statusVY = CalculateDT(vy1, vy2, cellData.DY, initialY, out vy, out dvydy, out dty);
            int statusVZ = CalculateDT(vz1, vz2, cellData.DZ, initialZ, out vz, out dvzdz, out dtz);

            int exitX = 0;
            int exitY = 0;
            int exitZ = 0;
            result.ExitFace = 0;
            int exitFace = 0;
            if (statusVX < 2 || statusVY < 2 || statusVZ < 2)
            {
                dt = dtx;
                if (vx < 0)
                { exitFace = 1; }
                else if (vx > 0)
                { exitFace = 2; }

                if (dty < dt)
                {
                    dt = dty;
                    if (vy < 0)
                    { exitFace = 3; }
                    else if (vy > 0)
                    { exitFace = 4; }
                }

                if (dtz < dt)
                {
                    dt = dtz;
                    if (vz < 0)
                    { exitFace = 5; }
                    else if (vz > 0)
                    { exitFace = 6; }
                }
            }

            double t = initialTime + dt;
            double x = 0;
            double y = 0;
            double z = 0;


            // If the maximumTime is less that the computed exit time, then calculate the particle
            // location at the maximumTime and set the final time equal to the maximum time.
            // Set the status to MaximumTimeReached and return.
            if (maximumTime < t)
            {
                dt = maximumTime - initialTime;
                t = maximumTime;
                x = NewXYZ(vx, dvxdx, vx1, vx2, dt, initialX, cellData.DX, statusVX);
                y = NewXYZ(vy, dvydy, vy1, vy2, dt, initialY, cellData.DY, statusVY);
                z = NewXYZ(vz, dvzdz, vz1, vz2, dt, initialZ, cellData.DZ, statusVZ);
                result.ExitFace = 0;
                result.FinalLocation = new ParticleLocation(cellNumber, x, y, z, t);
                result.Status = TrackingStatus.MaximumTimeReached;
            }
            // Otherwise, if the computed exit time is less than or equal to the maximum time, then
            // calculate the exit location and set the final time equal to the computed exit time.
            else
            {
                if (exitFace == 1 || exitFace == 2)
                {
                    x = 0;
                    y = NewXYZ(vy, dvydy, vy1, vy2, dt, initialY, cellData.DY, statusVY);
                    z = NewXYZ(vz, dvzdz, vz1, vz2, dt, initialZ, cellData.DZ, statusVZ);
                    if (exitFace == 2)
                    { x = 1; }
                }
                else if (exitFace == 3 || exitFace == 4)
                {
                    x = NewXYZ(vx, dvxdx, vx1, vx2, dt, initialX, cellData.DX, statusVX);
                    y = 0;
                    z = NewXYZ(vz, dvzdz, vz1, vz2, dt, initialZ, cellData.DZ, statusVZ);
                    if (exitFace == 4)
                    { y = 1; }
                }
                else if (exitFace == 5 || exitFace == 6)
                {
                    x = NewXYZ(vx, dvxdx, vx1, vx2, dt, initialX, cellData.DX, statusVX);
                    y = NewXYZ(vy, dvydy, vy1, vy2, dt, initialY, cellData.DY, statusVY);
                    z = 0;
                    if (exitFace == 6)
                    { z = 1; }
                }
                else
                {
                    // If it gets to this point, then something went wrong calculating the exit face number.
                    // Save the exit face number in the result and set the result status to Undefined and then return.
                    result.ExitFace = exitFace;
                    result.FinalLocation = null;
                    result.Status = TrackingStatus.Undefined;
                    return result;
                }

                // Record the exit face, and the final location in the result. Set the status to ExitAtCellFace and return.
                result.ExitFace = exitFace;
                result.InternalExitFace = false;
                if (result.ExitFace > 0)
                {
                    if (cellData.GetConnection(result.ExitFace) < 0)
                        result.InternalExitFace = true;
                }
                result.FinalLocation = new ParticleLocation(cellNumber, x, y, z, t);
                result.Status = TrackingStatus.ExitAtCellFace;

            }

            return result;

        }

        #endregion

        #region Private Members
        private int CalculateDT(double v1, double v2, double dx, double xL, out double v, out double dvdx, out double dt)
        {
            // Initialize properties
            dt = 1.0E+20;
            double v2a = Math.Abs(v2);
            double v1a = Math.Abs(v1);
            double dv = v2 - v1;
            double dva = Math.Abs(dv);
            double x = xL * dx;

            // Check for a uniform zero velocity in this direction
            // If so, set dt = 1.0E+20 and return with status = 2
            double tol = 1.0e-15;
            if (v2a < tol && v1a < tol)
            {
                v = 0;
                dvdx = 0;
                return 2;
            }

            // Check for a uniform non-zero velocity in this direction. 
            double vv = v1a;
            if (v2a > vv) vv = v2a;
            double vvv = dva / vv;
            if (vvv < 1.0e-4)
            {
                double zro = 1.0e-15;
                double zrom = -zro;
                v = v1;
                if (v1 > zro) dt = (dx - x) / v1;
                if (v1 < zrom) dt = -x / v1;
                dvdx = 0;
                return 1;
            }

            // Velocity has a linear variation.
            // Compute velocity corresponding to particle position
            dvdx = dv / dx;
            v = (1.0 - xL) * v1 + xL * v2;

            // If flow is into the cell from both sides ther is no outflow, so set status = 3 and return
            bool noOutflowFace = true;
            if (v1 < 0.0) noOutflowFace = false;
            if (v2 > 0.0) noOutflowFace = false;
            if (noOutflowFace)
            {
                return 3;
            }
            
            // If flow is out of the cell on both sides, find the location of the divide and compute travel time to the exit face.
            // Return with status = 0.
            if (v1 <= 0.0 && v2 >= 0.0)
            {
                if (Math.Abs(v) <= 0.0)
                {
                    v = 1.0e-20;
                    if (v2 <= 0.0) v = -v;
                }
            }

            double vr1 = v1 / v;
            double vr2 = v2 / v;
            double vr = vr1;
            if (vr <= 0.0) vr = vr2;
            double v1v2 = v1 * v2;
            if (v1v2 > 0.0)
            {
                if (v > 0.0) vr = vr2;
                if (v < 0.0) vr = vr1;
            }
            dt = Math.Log(vr) / dvdx;
            return 0;

        }
        private double NewXYZ(double v, double dvdx, double v1, double v2, double dt, double x, double dx, int velocityProfileStatus)
        {
            double newX = x;
            switch (velocityProfileStatus)
            {
                case 1:
                    newX += v1 * dt / dx;
                    break;
                case 2:
                    // return newX = x.
                    break;
                default:
                    if (v != 0)
                    {
                        newX += v * (Math.Exp(dvdx * dt) - 1.0) / dvdx / dx;
                    }
                    break;
            }
            if (newX < 0.0) newX = 0.0;
            if (newX > 1.0) newX = 1.0;

            return newX;
        }
        #endregion
    }
}
