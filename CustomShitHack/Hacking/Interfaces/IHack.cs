using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.CustomStuffHack.Hacking
{
    internal interface IHack
    {
        // Update and draw
        void OnUpdate(object sender, EventArgs args);
        void OnPostUpdate(object sender, EventArgs args);
        void OnDraw(object sender, OnDrawEventArgs args);

        // Disabling/Enabling
        void OnEnable();
        void OnDisable();
    }
}
