using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(Collider))]
public class Vehicle : MonoBehaviour {
    [SerializeField] List<Rigidbody> _rbs = new List<Rigidbody>(1);
    [SerializeField] float degThreshold = 20;
    [SerializeField] float getoffDistanceThreshold = 2;

    void Awake() {
        this.FixedUpdateAsObservable()
            .Select(_ => transform.position)
            .Pairwise()
            .Select(p=>p.Current - p.Previous)
            .Where(_ => _rbs.Count != 0)
            .Subscribe(delta => {
                for (int i = _rbs.Count-1; i != -1; i--) {
                    if ((_rbs[i].position - transform.position).sqrMagnitude > (getoffDistanceThreshold * getoffDistanceThreshold)) {
                        _rbs.RemoveAt(i);
                        continue;
                    }
                    _rbs[i].MovePosition(_rbs[i].position + delta);
                }
            }).AddTo(this);
    }
    void OnCollisionEnter(Collision other) {
        if (Vector2.Dot(other.contacts[0].normal, -Vector3.up) >= Mathf.Cos(degThreshold * Mathf.PI / 360f)) {
            _rbs.Add(other.rigidbody);
        }
    }
    void OnCollisionExit(Collision other) {
        _rbs.Remove(other.rigidbody);
    }

}
