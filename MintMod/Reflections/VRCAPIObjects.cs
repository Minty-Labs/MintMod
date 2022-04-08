using VRC.Core;

namespace MintMod.Reflections.VRCAPI {
    public class AvatarObject {
        public string name { get; set; }
        public string id { get; set; }
        public string authorId { get; set; }
        public string authorName { get; set; }
        public string assetUrl { get; set; }
        public string thumbnailImageUrl { get; set; }
        public ApiModel.SupportedPlatforms supportedPlatforms { get; set; }
        public string releaseStatus { get; set; }
        public string description { get; set; }
        public int version { get; set; }
        
        public ApiAvatar ToApiAvatar() {
            return new ApiAvatar {
                name = name,
                id = id,
                authorId = authorId,
                authorName = authorName,
                assetUrl = assetUrl,
                thumbnailImageUrl = thumbnailImageUrl,
                supportedPlatforms = supportedPlatforms,
                description = description,
                releaseStatus = releaseStatus,
                version = version
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
                supportedPlatforms = avatar.supportedPlatforms,
                description = avatar.description,
                releaseStatus = avatar.releaseStatus,
                version = avatar.version
            };
        }

        public AvatarObject(ApiAvatar avtr) {
            name = avtr.name;
            id = avtr.id;
            authorId = avtr.authorId;
            authorName = avtr.authorName;
            assetUrl = avtr.assetUrl;
            thumbnailImageUrl = avtr.thumbnailImageUrl;
            supportedPlatforms = avtr.supportedPlatforms;
            description = avtr.description;
            releaseStatus = avtr.releaseStatus;
            version = avtr.version;
        }

        public AvatarObject() { }
    }
}