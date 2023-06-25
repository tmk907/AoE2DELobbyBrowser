using AoE2DELobbyBrowser.WebApi.Reliclink;

namespace AoE2DELobbyBrowser.Tests
{
    public class Tests
    {
        private string _options = @"eNo9UttuwyAM/Zd9QZoL7+lgWzoBSwet8rpoigTqZdqqBL5+JjbhycL2udh+Os59C6+U5la0M0S8ZXKiP3d/7FPIfdBOzpgX7AvztYr3Zp8iPszatSKFyngmW8xrc39gT8/U8Tf97QDzR4lGq7dTfSoOp/dCNZ++2xm/nM35cP0wBUec24zcdtFuDMRT6Eu98mhugcemsIHaP9mTJrdpYipaxIqAtf7ZUrsOdTrLvrGnTD2ocwLvcsUEP10KpIF5mOTFIq+RmbdWPPFiLOOmoQS+hTTU2lni80xhvpIc8qu3EbwJ8ilgxpZ0igj/L8gPs4vtK8bg2XQ7qgEvQuD/yDTOfNGA/bzmx5CwJW8zZpN3JB14Qt0B9l7SvoJ0t4BaOuAf5v204iQtQVE9eA65HuZOPjrQMtCOugpuBT3HHnQNa61OfZz445BvpFIm78svebcyjkziXTYwS8IdS331dGMwB5xlAfop7+fcr6CfdltpnvFttd1o9NsNwy5oPhPswtNtdVlfs/bjHdb6cno8/QOGVPVe";
        private string _decoded = "G\u0004\0\0\061:4\u0003\0\0\00:2\u0004\0\0\062:n\u0005\0\0\092:30\u0003\0\0\01:n\u0004\0\0\087:y\u0004\0\0\060:0\u0004\0\0\059:0\u0004\0\0\089:n\u0003\0\0\04:5\u001b\0\0\052:j4NN4uxWBU+CyJB5NLVMbg==\u0003\0\0\05:0\u0005\0\0\051:72\u0004\0\0\064:n\u0004\0\0\085:0\u0005\0\0\095:-1\u0004\0\0\086:y\u0004\0\0\06:75\u0003\0\0\07:0\u0004\0\0\056:2\u0004\0\0\065:y\u0004\0\0\066:y\u0003\0\0\08:2\u0003\0\0\09:0\b\0\0\010:10895\u0004\0\0\093:0\u0005\0\0\084:-1\u0005\0\0\083:-1\u0004\0\0\067:1\u0004\0\0\068:5\u0004\0\0\069:5\u0004\0\0\070:1\u0005\0\0\071:10\u0005\0\0\012:50\u0004\0\0\013:1\u0005\0\0\014:70\u0006\0\0\015:125\u0004\0\0\016:1\u0004\0\0\017:8\u0004\0\0\018:1\b\0\0\072:10000\u0004\0\0\019:0\u0004\0\0\020:1\u0005\0\0\021:60\u0004\0\0\022:2\u0005\0\0\023:60\u0006\0\0\073:125\u0005\0\0\024:20\u0004\0\0\025:1\u0005\0\0\026:62\u0004\0\0\027:3\u0004\0\0\074:8\u0006\0\0\028:200\u0004\0\0\036:0\u0004\0\0\075:y\u0004\0\0\091:n\u0004\0\0\037:3\u0004\0\0\097:2\u0004\0\0\076:y\u0004\0\0\055:1\u0004\0\0\041:2\u0004\0\0\090:n\u0004\0\0\077:y\u0004\0\0\078:y\u0004\0\0\057:0\u0004\0\0\079:n\u0004\0\0\080:0\u0004\0\0\081:9\u0004\0\0\082:0\u0004\0\0\098:y\u0005\0\0\058:en";

        [Test]
        public void DecodeOptions_should_decode_options()
        {
            var decoded = OptionsDecoder.DecodeOptions(_options);
            Assert.That(decoded, Is.EqualTo(_decoded));
        }

        [Test]
        public void DecodedToDict_should_create_dictionary()
        {
            var dict = OptionsDecoder.DecodedToDict(_decoded);

            Assert.That(dict.Count, Is.EqualTo(71));
            Assert.That(dict[58], Is.EqualTo("en"));
            Assert.That(dict[61], Is.EqualTo("4"));
        }
    }
}