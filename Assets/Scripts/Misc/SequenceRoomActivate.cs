using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SequenceRoomActivate : ISequenceEvent {
    public void StartEvent(Action complete_callback) {
        // @TODO: actually don't need this...
        var seq = DOTween.Sequence();
        seq.SetDelay(1.0f).OnComplete(() => {
            var active_room = RoomManager.GetActiveRoom();
            active_room.ActivateRoom();
            complete_callback?.Invoke();
        });
    }
}
