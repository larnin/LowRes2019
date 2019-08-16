using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UpdateDarknessInsanityEvent : EventArgs
{
    public UpdateDarknessInsanityEvent(float _value)
    {
        value = _value;
    }

    public float value;
}