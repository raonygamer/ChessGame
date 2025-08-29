using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Components
{
    /// <summary>
    /// Represents how a UI element should size itself relative to its parent.
    /// </summary>
    public enum SizeMode
    {
        /// <summary>
        /// The element maintains a fixed size, independent of its parent's size.
        /// </summary>
        Fixed,

        /// <summary>
        /// The element uses anchors to determine its size relative to its parent.
        /// </summary>
        Stretch,

        /// <summary>
        /// The element sizes itself based on its content.
        /// </summary>
        Content
    }
}
