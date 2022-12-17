using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using UniRx;
using System;

    namespace Assembly.Components.Actors{
    public class DroneHatch : MonoBehaviour
    {
        [SerializeField] Transform batchPosition;
        [SerializeField]public AlarmMgr alarmMgr;
        [SerializeField] HostileDrone drone;
        [SerializeField] float launchSpeed;
        private bool _isLaunch;

        void Start()
        {
            Reset();
            alarmMgr.IsAlarm.Where(x => x==true).Subscribe(_ => Launch());
        }

        void Update(){
            if(_isLaunch){
                drone.gameObject.transform.position = Vector3.MoveTowards(drone.gameObject.transform.position, batchPosition.position, launchSpeed * Time.deltaTime);
                if(drone.gameObject.transform.position == batchPosition.position){
                    LaunchFinish();
                }
            }
        }

        void Launch(){
            drone.gameObject.SetActive(true);
            _isLaunch = true;
        }

        void LaunchFinish(){
            _isLaunch = false;
            DroneModuleEnable(true);
        }

        public void Reset(){
            DroneModuleEnable(false);
            drone.gameObject.transform.position = gameObject.transform.position;
            drone.gameObject.SetActive(false);
        }

        void DroneModuleEnable(bool onOff){
            drone.enabled = onOff;
            drone.emitter.enabled = onOff;
            drone.follow.enabled = onOff;
            drone.aim.enabled = onOff;
            drone.patrol.enabled = onOff;

            drone.gameObject.GetComponent<BoxCollider>().enabled = onOff;
            drone.gameObject.GetComponent<AlarmStopToReturn>().enabled = onOff;
        }

        Color color;
        void OnDrawGizmos()
        {
        if (color == default(Color))
        {
            color = UnityEngine.Random.ColorHSV(0, 1, 1, 1, 1, 1);
        }
        Gizmos.color = color;
        if (batchPosition)
        {
            Gizmos.DrawIcon(transform.position, "Check", true);
            GizmosEx.DrawArrow(transform.position,batchPosition.position);
        }
        else
        {
            Gizmos.DrawIcon(transform.position, "Cross", true);
        }
        }
    }
}