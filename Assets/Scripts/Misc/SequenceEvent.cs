using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISequenceEvent
{
    void StartEvent(Action complete_callback);
}
