using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.Modpath
{
    public class LabeledParticleCoodinate : ParticleCoordinate
    {
        #region Fields
        private int _ParticleID = 0;
        #endregion

        #region Public Members
        public int ParticleID
        {
            get { return _ParticleID; }
            set { _ParticleID = value; }
        }
        #endregion


    }
}
