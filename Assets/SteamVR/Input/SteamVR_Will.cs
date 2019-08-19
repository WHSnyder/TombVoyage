//======= Copyright (c) Valve Corporation, All rights reserved. ===============

using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

namespace Valve.VR
{
    /// <summary>
    /// This component simplifies the use of Pose actions. Adding it to a gameobject will auto set that transform's position and rotation every update to match the pose.
    /// Advanced velocity estimation is handled through a buffer of the last 30 updates.
    /// </summary>
    public class SteamVR_Will : SteamVR_Behaviour_Pose
    {

        public bool contact = false;
        public Vector3 contactNormal;


        public bool hold = false;

        public Transform player;
        private Rigidbody playerRig;

        private Vector3 lastPos;
        private Vector3 handForce;

        override
        protected void Start(){
            if (poseAction == null){
                Debug.LogError("<b>[SteamVR]</b> No pose action set for this component");
                return;
            }

            CheckDeviceIndex();

            if (origin == null)
                origin = this.transform.parent;

            playerRig = player.GetComponent<Rigidbody>();
        }

        private void SteamVR_Behaviour_Pose_OnUpdate(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource){
            UpdateHistoryBuffer();

            UpdateTransform();

            if (onTransformUpdated != null)
                onTransformUpdated.Invoke(this, inputSource);
            if (onTransformUpdatedEvent != null)
                onTransformUpdatedEvent.Invoke(this, inputSource);
        }

        override
        protected void UpdateTransform(){

            CheckDeviceIndex();

            if (hold){
                transform.localPosition = poseAction[inputSource].localPosition;
                transform.localRotation = poseAction[inputSource].localRotation;

                player.position += -1 * (transform.position - lastPos);

                transform.position = lastPos;
            }
            else if (contact){
                transform.localPosition = poseAction[inputSource].localPosition;
                transform.localRotation = poseAction[inputSource].localRotation;

                Vector3 diff = Vector3.Normalize( transform.position - lastPos );
                float dot = Vector3.Dot(diff, contactNormal);

                Debug.Log("Pushing off with dot of: " + dot);

                if (dot < -0.7){
                    player.GetComponent<Rigidbody>().velocity = contactNormal;
                }

                lastPos = transform.position;              
            }
            else {
                transform.localPosition = poseAction[inputSource].localPosition;
                transform.localRotation = poseAction[inputSource].localRotation;

                lastPos = transform.position;
            }
        }
    }
}