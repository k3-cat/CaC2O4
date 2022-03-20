using System.Security.Cryptography;
using System.Text;
using Aliyun.OSS;

namespace CaC2O4.Providers;

public interface IAliOss {
    Uri NewDocUpload(DateTimeOffset now, String key);
    void DeleteFile(String key);
    void DeleteDirectory(String dMark);
    Boolean VerifySignature(HttpContext ctx);
}

public class AliOss : IAliOss {
    readonly OssClient _client;
    readonly String _bracket;
    readonly String _pubKey;

    public AliOss(IConfiguration conf) {
        _client = new OssClient(conf["AliOss:Endpoint"], conf["AliOss:AccessKeyId"], conf["AliOss:AccessKeySecret"]);
        _bracket = conf["AliOss:Bracket"];
        _pubKey = conf["AliOss:PubKey"];
    }

    public Uri NewDocUpload(DateTimeOffset now, String key) {
        var req = new GeneratePresignedUriRequest(_bracket, key, SignHttpMethod.Put) {
            Expiration = now.AddMinutes(15).DateTime,
        };
        return _client.GeneratePresignedUri(req);
    }

    public void DeleteFile(String key) {
        _client.DeleteObject(_bracket, key);
    }

    public void DeleteDirectory(String dMark) {
        ObjectListing result;
        var nextMarker = String.Empty;
        var keys = new List<String>();
        do {
            var listReq = new ListObjectsRequest(_bracket) {
                Marker = nextMarker,
                Prefix = dMark,
            };
            result = _client.ListObjects(listReq);
            nextMarker = result.NextMarker;
            keys.AddRange(result.ObjectSummaries.Select(x => x.Key));
        } while (result.IsTruncated);

        var delReq = new DeleteObjectsRequest(_bracket, keys, true);
        _client.DeleteObjects(delReq);
    }

    public Boolean VerifySignature(HttpContext ctx) {
        var authorization = Convert.FromBase64String(ctx.Request.Headers.Authorization);

        var rawReq = new StringBuilder();
        rawReq.Append(ctx.Request.Path);
        rawReq.Append(ctx.Request.QueryString.ToUriComponent());
        rawReq.Append('\n');
        rawReq.Append(ctx.Request.BodyReader);

        using var rsa = RSA.Create();
        rsa.ImportFromPem(_pubKey);
        return rsa.VerifyData(Encoding.UTF8.GetBytes(rawReq.ToString()), authorization, HashAlgorithmName.MD5, RSASignaturePadding.Pkcs1);
    }
}
