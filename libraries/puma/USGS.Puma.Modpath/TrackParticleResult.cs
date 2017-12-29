using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.Modpath
{
    public class TrackParticleResult
    {
        #region Fields
        private TrackCellStatus _Status = TrackCellStatus.Undefined;
        private int _ParticleID = 0;
        private ParticlePath _ParticlePath = null;
        private ParticleLocation _NextLocation = null;

        #endregion

        #region Constructors
        public TrackParticleResult()
        {
            ParticleID = 0;
            Status = TrackCellStatus.Undefined;
            ParticlePath = new ParticlePath();
        }

        public TrackParticleResult(int particleID, ParticlePath particlePath)
            : this()
        {
            if (particlePath != null)
            { ParticlePath = particlePath; }
        }

        #endregion

        #region Public Methods
        public int ParticleID
        {
            get { return _ParticleID; }
            set { _ParticleID = value; }
        }

        public ParticlePath ParticlePath
        {
            get { return _ParticlePath; }
            set { _ParticlePath = value; }
        }

        public ParticleLocation NextLocation
        {
            get { return _NextLocation; }
            set { _NextLocation = value; }
        }

        public TrackCellStatus Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        #endregion

    }
}
