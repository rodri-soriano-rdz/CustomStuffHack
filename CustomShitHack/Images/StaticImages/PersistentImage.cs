using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.CustomShitHack.ImageSystem
{
    internal class PersistentImage : BaseImage
    {
        IList<TeamHat> m_createdHats = new List<TeamHat>();

        internal PersistentImage(ImageSection firstSection) : base(firstSection)
        {

        }

        public override List<TeamHat> CreateInstance(Vec2 pos)
        {
            return base.CreateInstance(pos);
        }
    }
}
