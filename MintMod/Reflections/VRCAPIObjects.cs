using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRC.Core;

namespace MintMod.Reflections.VRCAPI {
    public class Platforms {
        public int standalonewindows { get; set; }
        public int android { get; set; }
    }

    public class World {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool featured { get; set; }
        public string authorId { get; set; }
        public string authorName { get; set; }
        public int capacity { get; set; }
        public List<string> tags { get; set; }
        public string releaseStatus { get; set; }
        public string imageUrl { get; set; }
        public string thumbnailImageUrl { get; set; }
        public string assetUrl { get; set; }
        public int version { get; set; }
        public string organization { get; set; }
        public object previewYoutubeId { get; set; }
        public int favorites { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime publicationDate { get; set; }
        public string labsPublicationDate { get; set; }
        public int visits { get; set; }
        public int popularity { get; set; }
        public int heat { get; set; }
    }

    public class WorldInstanceLite {
        public string RoomID { get; set; }
        public string id { get; set; }
        public string IdOnly { get; set; }
        public string IdWithTags { get; set; }
        public string name { get; set; }
        public DateTime TimeOfJoin { get; set; }
    }

    public class WorldInstanceResponse {
        public string id { get; set; }
        public string name { get; set; }

        [JsonProperty(PropertyName = "private")]
        public List<WorldInstanceUserResponse> privateUsers { get; set; }

        public List<WorldInstanceUserResponse> friends { get; set; }
        public List<WorldInstanceUserResponse> users { get; set; }
        public string hidden { get; set; }
        public string nonce { get; set; }
    }

    public class WorldInstanceUserResponse {
        public string id { get; set; }
        public string username { get; set; }
        public string displayName { get; set; }
        public string currentAvatarImageUrl { get; set; }
        public string currentAvatarThumbnailImageUrl { get; set; }
        public List<string> tags { get; set; }
        public string developerType { get; set; }
        public string status { get; set; }
        public string statusDescription { get; set; }
        public string networkSessionId { get; set; }
    }

    public class User {
        public string id { get; set; }
        public string username { get; set; }
        public string displayName { get; set; }
        public string bio { get; set; }
        public string currentAvatarImageUrl { get; set; }
        public string currentAvatarThumbnailImageUrl { get; set; }
        public string userIcon { get; set; }
        public string last_platform { get; set; }
        public string status { get; set; }
        public string state { get; set; }
        public List<string> tags { get; set; }
        public string developerType { get; set; }
        public bool isFriend { get; set; }
    }

    public class AvatarObject {
        public string name { get; set; }
        public string id { get; set; }
        public string authorId { get; set; }
        public string authorName { get; set; }
        public string assetUrl { get; set; }
        public string thumbnailImageUrl { get; set; }
        public string supportedPlatforms { get; set; }
        public string releaseStatus { get; set; }
        public string description { get; set; }

        //public string unityVersion { get; set; }
        public int version { get; set; }

        //public bool featured { get; set; }
        public ApiAvatar ToApiAvatar() {
            return new ApiAvatar {
                name = name,
                id = id,
                authorId = authorId,
                authorName = authorName,
                assetUrl = assetUrl,
                thumbnailImageUrl = thumbnailImageUrl,
                supportedPlatforms = supportedPlatforms == "All " ? ApiModel.SupportedPlatforms.All : ApiModel.SupportedPlatforms.StandaloneWindows,
                description = description,
                releaseStatus = releaseStatus,
                version = version,
                //unityVersion = unityVersion,
                //assetVersion = new AssetVersion(unityVersion, 0),
            };
        }

        public static ApiAvatar ApiAvatar(AvatarObject avatar) {
            return new ApiAvatar {
                name = avatar.name,
                id = avatar.id,
                authorId = avatar.authorId,
                authorName = avatar.authorName,
                assetUrl = avatar.assetUrl,
                thumbnailImageUrl = avatar.thumbnailImageUrl,
                supportedPlatforms = avatar.supportedPlatforms == "All" ? ApiModel.SupportedPlatforms.All : ApiModel.SupportedPlatforms.StandaloneWindows,
                description = avatar.description,
                releaseStatus = avatar.releaseStatus,
                version = avatar.version,
                //unityVersion = unityVersion,
                //assetVersion = new AssetVersion(unityVersion, 0),
            };
        }

        public AvatarObject(ApiAvatar avtr) {
            name = avtr.name;
            id = avtr.id;
            authorId = avtr.authorId;
            authorName = avtr.authorName;
            assetUrl = avtr.assetUrl;
            thumbnailImageUrl = avtr.thumbnailImageUrl;
            supportedPlatforms = avtr.supportedPlatforms.ToString();
            description = avtr.description;
            releaseStatus = avtr.releaseStatus;
            version = avtr.version;
            //unityVersion = avtr.unityVersion;
        }

        /*
        public AvatarObject(Il2CppSystem.Collections.Generic.Dictionary<string, Il2CppSystem.Object> avtr)
        {
            name = avtr["name"].ToString();
            id = avtr["id"].ToString();
            authorId = avtr["authorId"].ToString();
            authorName = avtr["authorName"].ToString();
            assetUrl = avtr["assetUrl"].ToString();
            thumbnailImageUrl = avtr["thumbnailImageUrl"].ToString();
            supportedPlatforms = "All";
            description = avtr["description"].ToString();
            releaseStatus = avtr["releaseStatus"].ToString();
            //unityVersion = avtr.unityVersion;
        }
        */

        public AvatarObject() { }
    }

    public class WorldInstance {
        public string id { get; set; }
        public string location { get; set; }
        public string instanceId { get; set; }
        public string name { get; set; }
        public string worldId { get; set; }
        public string type { get; set; }
        public string ownerId { get; set; }
        public List<string> tags { get; set; }
        public bool active { get; set; }
        public bool full { get; set; }
        public int n_users { get; set; }
        public int capacity { get; set; }
        public Platforms platforms { get; set; }
        public string shortName { get; set; }
        public World world { get; set; }
        public List<User> users { get; set; }
        public string nonce { get; set; }
        public string clientNumber { get; set; }
        public string photonRegion { get; set; }
        public bool canRequestInvite { get; set; }
        public bool permanent { get; set; }
        public string @private { get; set; }
    }

    public class Details {
        public string worldId { get; set; }
        public string worldName { get; set; }
    }

    public class Notification {
        public string id { get; set; }
        public string type { get; set; }
        public string senderUserId { get; set; }
        public string senderUsername { get; set; }
        public string receiverUserId { get; set; }
        public string message { get; set; }
        public Details details { get; set; }
        public DateTime created_at { get; set; }
    }
    public class FileData {
        public string fileName { get; set; }
        public string url { get; set; }
        public string md5 { get; set; }
        public int sizeInBytes { get; set; }
        public string status { get; set; }
        public string category { get; set; }
        public string uploadId { get; set; }
    }

    public class Delta {
        public string fileName { get; set; }
        public string url { get; set; }
        public string md5 { get; set; }
        public int sizeInBytes { get; set; }
        public string status { get; set; }
        public string category { get; set; }
        public string uploadId { get; set; }
    }

    public class Signature {
        public string fileName { get; set; }
        public string url { get; set; }
        public string md5 { get; set; }
        public int sizeInBytes { get; set; }
        public string status { get; set; }
        public string category { get; set; }
        public string uploadId { get; set; }
    }

    public class Version {
        public int version { get; set; }
        public string status { get; set; }
        public DateTime created_at { get; set; }
        public FileData file { get; set; }
        public Delta delta { get; set; }
        public Signature signature { get; set; }
    }

    public class ImageFile {
        public string id { get; set; }
        public string name { get; set; }
        public string ownerId { get; set; }
        public string mimeType { get; set; }
        public string extension { get; set; }
        public List<object> tags { get; set; }
        public List<Version> versions { get; set; }
    }

    public class VRCList {
        public VRCList(Transform parent, string name, int Position = 0) {
            GameObject = UnityEngine.Object.Instantiate<GameObject>(PublicAvatarList.gameObject, parent);
            GameObject.GetComponent<UiAvatarList>().field_Public_Category_0 = UiAvatarList.Category.SpecificList;
            UiVRCList = GameObject.GetComponent<UiVRCList>();
            Text = GameObject.transform.Find("Button").GetComponentInChildren<Text>();
            GameObject.transform.SetSiblingIndex(Position);

            UiVRCList.clearUnseenListOnCollapse = false;
            UiVRCList.usePagination = false;
            UiVRCList.hideElementsWhenContracted = false;
            UiVRCList.hideWhenEmpty = false;
            UiVRCList.field_Protected_Dictionary_2_Int32_List_1_ApiModel_0.Clear();

            GameObject.SetActive(true);
            GameObject.name = name;
            Text.text = name;
        }

        public GameObject GameObject;
        public UiVRCList UiVRCList;
        public Text Text;

        //public void RenderElement(Il2CppSystem.Collections.Generic.List<ApiModel> ModelList)
        //{
        //    UiVRCList.Method_Protected_Void_List_1_T_Int32_Boolean_VRCUiContentButton_0<ApiModel>(ModelList, 0, true);
        //}
        public void RenderElement(Il2CppSystem.Collections.Generic.List<ApiAvatar> AvatarList) {
            UiVRCList.Method_Protected_Void_List_1_T_Int32_Boolean_VRCUiContentButton_0<ApiAvatar>(AvatarList, 0, true);
        }

        //public void RenderElement(Il2CppSystem.Collections.Generic.List<APIUser> UserList)
        //{
        //    UiVRCList.Method_Protected_Void_List_1_T_Int32_Boolean_VRCUiContentButton_0<APIUser>(UserList, 0, true);
        //}

        private static GameObject PublicAvatarList = GameObject.Find("/UserInterface/MenuContent/Screens/Avatar/Vertical Scroll View/Viewport/Content/Public Avatar List");
    }

    internal enum MenuButtonType {
        PlaylistButton,
        AvatarFavButton,
        HeaderButton
    }

    public enum MenuType {
        UserInfo,
        AvatarMenu,
        SettingsMenu,
        SocialMenu,
        WorldMenu
    }
}
