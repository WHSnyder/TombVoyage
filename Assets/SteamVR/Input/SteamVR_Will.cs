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

        Vector3 lastPlayer = Vector3.zero;

        public Vector3 velAccum  = Vector3.zero;

        public bool hold = false;

        public Transform player;
        private Rigidbody playerRig;

        public Vector3 pushVel;

        private Vector3 lastPos;
        private Vector3 lastPosLocal;

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

                Vector3 diff = (transform.position - lastPos);
                float normedDot = Vector3.Dot(Vector3.Normalize(diff), contactNormal);

               // Debug.Log("Pushing off with dot of: " + normedDot);
               // Debug.Log("Diff vector: " + diff.ToString());// + " normal vector: " + contactNormal.ToString() );

                Vector3 playerDiff = this.transform.position - lastPlayer; //player.transform.position - lastPlayer;

                if (normedDot < 0.0){
                    //pushVel =  playerDiff/Time.deltaTime;
                    if (Vector3.Magnitude(playerDiff) > .1)
                    {

                        velAccum = playerDiff;
                    }

                    Debug.Log( "velcoity accumulated: " + velAccum);
                }

                lastPos = transform.position;
                lastPosLocal = transform.localPosition;
            }
            else {
                transform.localPosition = poseAction[inputSource].localPosition;
                transform.localRotation = poseAction[inputSource].localRotation;

                lastPos = transform.position;
                lastPosLocal = transform.localPosition;
            }

            lastPlayer = player.transform.position;
        }
    }
}