using Core.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Nodes.Interfaces
{
    public interface IControlNode
    {
        RectTransform Transform { get; }
    }
}
