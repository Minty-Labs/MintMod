using System.Collections;
using UnityEngine;
using VRC.Animation;
using VRC;
using MelonLoader;
using MintMod.UserInterface.QuickMenu;
using UnityEngine.XR;

namespace MintMod.Functions {
    class Movement : MintSubMod {
        public override string Name => "Movement";
        public override string Description => "Movement settings.";

        public static bool FlightEnabled, NoclipEnabled, SpeedModified;
        private static GameObject localPlayer;
        private static Vector3 originalGravity = new(0, -9.81f, 0);
        private static Player player;
        private static VRCMotionState motionState;
        private static InputStateController stateController;
        public static float finalSpeed = 1f;

		internal override void OnUserInterface() {
            MelonCoroutines.Start(Flying());
            MelonCoroutines.Start(Speed());
        }

		private static IEnumerator Flying() {
			while (true) {
				if (RoomManager.field_Internal_Static_ApiWorld_0 != null) {
					//if (!FlightEnabled) yield break;
					if (localPlayer == null && VRCPlayer.field_Internal_Static_VRCPlayer_0 != null &&
						VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject != null)
						localPlayer = VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject;

					if (motionState == null && localPlayer != null)
						motionState = localPlayer.GetComponent<VRCMotionState>();

					if (stateController == null && localPlayer != null)
						stateController = localPlayer.GetComponent<InputStateController>();

					if (player == null && VRCPlayer.field_Internal_Static_VRCPlayer_0 != null)
						player = VRCPlayer.field_Internal_Static_VRCPlayer_0._player;
					else {
						if (FlightEnabled && originalGravity == Vector3.zero) {
							originalGravity = Physics.gravity;
							Physics.gravity = Vector3.zero;
						}
						if (!FlightEnabled && originalGravity != Vector3.zero) {
							Physics.gravity = originalGravity;
							originalGravity = Vector3.zero;
						}

						if (!FlightEnabled && NoclipEnabled)
							NoclipEnabled = false;

						if (FlightEnabled) {
							Transform transform = Camera.main.transform;
							if (XRDevice.isPresent) {
								if (Input.GetAxis("Vertical") != 0f)
									localPlayer.transform.position += localPlayer.transform.forward * Time.deltaTime * Input.GetAxis("Vertical") * 12f * finalSpeed;

								if (Input.GetAxis("Horizontal") != 0f)
									localPlayer.transform.position += localPlayer.transform.right * Time.deltaTime * Input.GetAxis("Horizontal") * 12f * finalSpeed;

								if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") != 0f)
									localPlayer.transform.position += new Vector3(0f, Time.deltaTime * Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical")) * finalSpeed * 10f;
							} else {
								if (Input.GetAxis("Vertical") != 0f)
									localPlayer.transform.position += transform.transform.forward * Time.deltaTime * Input.GetAxis("Vertical") * ((Input.GetKey(KeyCode.LeftShift) ? 14f : 12f) * finalSpeed);

								if (Input.GetAxis("Horizontal") != 0f)
									localPlayer.transform.position += transform.transform.right * Time.deltaTime * Input.GetAxis("Horizontal") * ((Input.GetKey(KeyCode.LeftShift) ? 14f : 12f) * finalSpeed);

								if (Input.GetKey(KeyCode.Q))
									localPlayer.transform.position -= new Vector3(0f, Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? 14f : 12f) * finalSpeed, 0f);

								if (Input.GetKey(KeyCode.E))
									localPlayer.transform.position += new Vector3(0f, Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? 14f : 12f) * finalSpeed, 0f);
							}

							if (motionState != null) {
								motionState.Reset();
								if (motionState.field_Private_CharacterController_0 != null)
									motionState.field_Private_CharacterController_0.enabled = !NoclipEnabled;
							}

							//if (stateController != null && !NoclipEnabled) stateController.ResetLastPosition();
						}
					}
				}
				yield return new WaitForEndOfFrame();
			}
		}

		public static void Fly(bool toggle) {
			if (toggle) {
				FlightEnabled = true;
				SpeedModified = true;
				// MintUserInterface.FlightSpeedSlider.Active = true;
			} else {
				FlightEnabled = false;
				SpeedModified = false;
				// MintUserInterface.FlightSpeedSlider.Active = false;

				if (NoclipEnabled) {
					NoclipEnabled = false;
					SpeedModified = false;
					if (motionState != null) {
						motionState.Reset();
						if (motionState.field_Private_CharacterController_0 != null)
							motionState.field_Private_CharacterController_0.enabled = !NoclipEnabled;
					}
				}
				if (MintUserInterface.MainQMNoClip != null)
                    MintUserInterface.MainQMNoClip.Toggle(false);
				if (MintUserInterface.MintQANoClip != null)
                    MintUserInterface.MintQANoClip.Toggle(false);
			}
			if (MintUserInterface.MainQMFly != null)
                MintUserInterface.MainQMFly.Toggle(toggle);
			if (MintUserInterface.MintQAFly != null)
                MintUserInterface.MintQAFly.Toggle(toggle);
		}

		public static void NoClip(bool toggle) {
			if (toggle) {
				NoclipEnabled = true;
				SpeedModified = true;
				if (!FlightEnabled) {
					FlightEnabled = true;
					SpeedModified = true;
                    if (MintUserInterface.MainQMFly != null)
                        MintUserInterface.MainQMFly.Toggle(true);
                    if (MintUserInterface.MintQAFly != null)
                        MintUserInterface.MintQAFly.Toggle(true);
				}
			} else {
				NoclipEnabled = false;
				SpeedModified = false;
			}
            if (MintUserInterface.MainQMNoClip != null)
                MintUserInterface.MainQMNoClip.Toggle(toggle);
            if (MintUserInterface.MintQANoClip != null)
                MintUserInterface.MintQANoClip.Toggle(toggle);
		}

        private static float originalWalkSpeed, originalRunSpeed, originalStrafeSpeed;

		private static IEnumerator Speed() {
			while (true) {
				if (RoomManager.field_Internal_Static_ApiWorld_0 == null) {
					originalRunSpeed = 0f;
					originalWalkSpeed = 0f;
					originalStrafeSpeed = 0f;
				}
				while (RoomManager.field_Internal_Static_ApiWorld_0 == null)
					yield return new WaitForSeconds(1f);
				yield return new WaitForEndOfFrame();
				if (VRCPlayer.field_Internal_Static_VRCPlayer_0 != null && VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCPlayerApi_0 != null && RoomManager.field_Internal_Static_ApiWorld_0 != null) {

					if (SpeedModified && (originalRunSpeed == 0f || originalWalkSpeed == 0f || originalStrafeSpeed == 0f)) {
						originalWalkSpeed = VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCPlayerApi_0.GetWalkSpeed();
						originalRunSpeed = VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCPlayerApi_0.GetRunSpeed();
						originalStrafeSpeed = VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCPlayerApi_0.GetStrafeSpeed();
					}
					if (!SpeedModified && originalRunSpeed != 0f && originalWalkSpeed != 0f && originalStrafeSpeed != 0f) {
						VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCPlayerApi_0.SetWalkSpeed(originalWalkSpeed);
						VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCPlayerApi_0.SetRunSpeed(originalRunSpeed);
						VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCPlayerApi_0.SetStrafeSpeed(originalStrafeSpeed);
						originalRunSpeed = 0f;
						originalWalkSpeed = 0f;
						originalStrafeSpeed = 0f;
						finalSpeed = 1f;
					}
					if (SpeedModified && originalWalkSpeed != 0f && originalRunSpeed != 0f && originalStrafeSpeed != 0f) {
						VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCPlayerApi_0.SetWalkSpeed(originalWalkSpeed * finalSpeed);
						VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCPlayerApi_0.SetRunSpeed(originalRunSpeed * finalSpeed);
						VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCPlayerApi_0.SetStrafeSpeed(originalStrafeSpeed * finalSpeed);
					}
				}
			}
		}
	}
}
