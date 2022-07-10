using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerMaskExtention
{
    public static bool Includes(this LayerMask mask, int layer)
    {
        return (mask.value & 1 << layer) > 0;
    }
}
