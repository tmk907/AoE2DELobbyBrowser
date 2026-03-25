using AoE2DELobbyBrowser.WebApi.Reliclink;
using System.Text;
using System.Text.RegularExpressions;

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

        [Test]
        public void Test1()
        {
            var options = "eNq91VtPwjAUAGB/S5+noe1GKYkvCCVbBGE3AeNDYSUsu7Bs0wSN/127aMY0iykPezon7dmafDk9hUh7egdZftyHsTDT/fEmDMAQ6giTHtVAUfIyPKbm+GtNA6XgiUwNDez57mejp4Gc70RVo8s8je7Fq4i/d9JoxsvdwT1lVcm1/E+YiIXIWc4TMXOqurCwBQ9O1THy0JeiWk5Eyce85GAIzIhh22Ojpcfw0q/iZOnbMo5813rYTq3STuOD/xbb6yiYzv1s7URZuPWtYuMxJuuWq+Cuit7G2bJ4NZtY1feuP6ELx7wFH9pfDEJ1ghsUek3Rb6PAqFsKW5nioE4BCaLQ+KWBaw3S2hgdazjKGrG6hkH7kNImxqDGGLRhoEG3GK4yRnZBa+iYIF12/ZkGrTVoq0bHM8NX1XCZugaGfUphc372agyZt1wU3K2Gp6wxUtdACCJkNDXOnhOZtwxR0q3Go7KGdYEGRQiRpoZxpoFab0rHQ3QNVTXm/2g8X30CCpKljg==";

            var spectateOpt = "eNpFUtlOxDAM/Be+oPcbD1lSYCWc0CUF7etWKCIVdMWhtPl6nNhtK1UaxfbM+Lh5sZ3ArwAzBeERSdGA4Dd3/TtEKFuP8XnDcn0fF+3AU13bXIirUuFaHyKSZ6+daCNUZlx5K22uf1TTNXD4iW85an1eHn7fTDEuQ3H6Ntn18b19PZ6KoX5p70/PJpPEMwXS7mfthox1Mv01Jh1tzrsPM3n2meuPbKZcW6y5ytgGuH/kKsnTsPWvzNZHrLlP/HKawQlJWpO/EzQT7fCXA+ePAfvm/K55Mi1jiJi0Hep0rC2nnHVq7XqKB9xD10dYxtmDSBi1p5z9zRC2nWTo3x9swjl6WRRxz8pN3HdbamcTN8S+O0vcGAcp/MoZe6L8Y+wp+QbXNor3CO6IM0teFtQvQJyJJ8S59JyPe93y+7WPBb0UPO9FBdxj0hnQu32gXBvrWB8K/VnxLPrmnfqpAW+H4+V6WzreFvHWyLsw715vtvos3jnvyO/8w3ozpZbrzvuNX4UR42leFeD+1T6vCrZ52WX3Y1Hv9vbmH5UhBCo=";
            var nospectateOpt = "eNpFUttu2zAM/Zd8gW8x0Ic8KJNXZCilerEw5LXGoE5GJ68XyNbXjzIpx4ABQiTPheThanuBXwWDjyJgJEULgt/c/HVOoewC5pc9lvl9WrWDQH1d+0JYjYrz8ZwieQvaiS6FapgybqOH+Yt6+hbOH+mtRK7il4FFm/Lp5/Tw3tevunc//l2LuXvp5vp5KCTh+EDcZtFuLJin0G/NxqOH213HXjuV+k+xUK2t9N+JNdkW2D9i1aRp3P2rYfeRer5v+NIv4IQkLh++CZqJdvjLkeuniL65vm+fho5jSDFxO+TpmVv6knmO2hnKR9xDb1JYp9mD2GLk9iXrWyDuOylQfzjbLS5Ry6oIe1HOs++u1s5u2JB895awMQ9ShIyZPFH9JXnadIPrWsV7BHfBmW1aVuSvQNwIJ6a5GK7Hve71JvtYUUvF815V9JF4RtRuH6nWpj7mhyrvVEXT/iY/R8Db4Xydb0un2yLcI+KujHvvH0y+iSLdOe8o3PHHnK+1zDs3O76KE+Y3Dw1I/8l7axCLa+1612JR6+l0+A+gxAJY";
            var nospec2 = "eNpFUttu2zAM/Zd+gW/xWx/kyRkClFLdWkPzuHqDNnmNgl4gW18/yqQcAwYIkTwXknfPdhD4VTD6KAJGUrQg+M1dv7oUyj5gftljmd/nVTsI1Ne3r4TVqHg9dCmS56Cd6FOoxjnjNnq8flHP0EL3kd5K5Dr9OnaPTxd1+ln8eYd/Px7M8fryaj7rpxd1eRwLSTg+ELdZtJsK5in0W7Px6PF807HXzqX+WyxUayt9mVmTbYH9I1ZNmqbdvxp3H6nnuOFLv4ATkrh8+CZoJtrhLyeunyP65vqhfRh7jiHFxO2QZ2Bu6UvmOWhnKB9xD4NJYZ1mD2KLkduXrG+BuO+kQP2hs1tcopZVEfainGfffa2d3bAh+R4sYWMepAgZM3mi+lPytOkG17eK9wjuhDPbtKzIX4E4E05MczFcj3vd6032saKWiue9qugj8Uyo3X6nWpv6mB+qvFMVTfub/BwAb4fzdb4tnW6LcA+IuzLurX80+SaKdOe8o3DDn3K+1jLv3Oz4Ks6Y3zw0IP0n761BLK61602LRa3393f/AchgAsQ=";
            var spec1min = "eNpNUttugzAM/Zd+AXdpj7BkHZOSjDVMYm8bqtDCWpC6KpCvn4OTsDxZjn0utg8n3ZTwEiYnU2qISFmw0uXUfK9sSKiG/yXExOfHVSimsY8WX8PWl3Ez55WNSKeFKqkNuRw9bibkfMeepmDVzeZi4Fo4+VFn0hhxvOnP9/n4cXmoT8nH85t8ub7KiCDOpJG7XYTqI8cTiUu28QjZ7TpC7RiL72jB2iER19FpGgqGtQlgpaip9zpjrqbV4cehR3XFuWnRh6IrK5mNc/BZ24BJmJ+0MxidHgZ4WM/J9MtcLzM2Rm5uJqetA8zW8YwFx/+UET/7HjxT54nC7FuNfdRA/gn5m4Kb8ohxC1rq2NWAR0ox3xcCPS4CsB+3/3612IyUHjP3u2MKPKHuFe4gcfNZmZ3PpqUG/k5Xw4Zjtazc1YPn1dfzcEM1aOncbOsUbgg9mwZ0dVutsH3E8Zuw05RLf1vj4nfOTNhZLqTf2Qi4tcMN/fHe3yyeV8DOAr6B28Z5ZPudBPzU3i56GPS/mwIst29S71rInB/+ABluBcY=";
            var spec2min = "eNpFUttO5DAM/Re+oLeptA88pKSMkHCyhQQ0PFKhiHSZjLSgtPl6nNrtVKpkxfa52L55doPArwITkogYSdGC4Dd/+elyKPuI+XmP5fY+LdpDpL6+fSesRqXLocuRPEXtRZ9DZaYNt9Hm8kM9Qwvd//xWIlc/vHTHN/s0PVeXe/j68+/dutK8zufX4/f5rykk4YRI3HbWfiyYp9BfzcqjzemqY6+dSv1ZzFTrKn2eWJNrgf0jVk2axt2/MruP3HO/4sswgxeSuEK8EzQT7fGXI9dPCX1z/dA+mp5jyDFxe+QZmFuGknkO2lvKJ9zDYHNY59mDWGPkDiXrmyHtOylQf+zcGpeoZVGEPSsf2Hdfa+9WbMi+B0fYmAcp4oaZPVH9Q/a06gbft4r3CP4BZ7ZqWZC/AnEinJTnYrke97rX283HgloqnveiUkjEM6J2d6Ral/uYH6ptpyrZ9oP8HABvh/P1dls63xbhHhB3Ydxrv9n7i3znvKN4xR+3m6m13HZud3yVJsyvHhqQ4Zv31iAW17rlqsUh1+3tzS/tzALD";

            var dictSpect = OptionsDecoder.DecodedToDict(OptionsDecoder.DecodeOptions(spectateOpt));
            var dictNoSpect = OptionsDecoder.DecodedToDict(OptionsDecoder.DecodeOptions(nospectateOpt));
            var dictNoSpect2 = OptionsDecoder.DecodedToDict(OptionsDecoder.DecodeOptions(nospec2));
            var dictspec1min = OptionsDecoder.DecodedToDict(OptionsDecoder.DecodeOptions(spec1min));
            var dictspec2min = OptionsDecoder.DecodedToDict(OptionsDecoder.DecodeOptions(spec2min));

            foreach (var key in dictSpect.Keys.OrderBy(x => x))
            {
                if (dictSpect[key] != dictspec2min[key])
                {
                    System.Diagnostics.Debug.WriteLine($"{key}: {dictSpect[key]} :: {dictNoSpect[key]} :: {dictNoSpect2[key]} :: {dictspec1min[key]} :: {dictspec2min[key]}");
                }
            }
            /*
            5: 3 :: 0 :: 0 :: 0 :: 0
            52: flkVOi2sdkOJGxEHGg9HAQ== :: tYC19MKFOkCxNB2jKJDlJw== :: HtPOFsHkHk2UKPZWmKwEsg== :: q49cx438k0iZFfoHKfGE2g== :: DAPFeDdKjE2oemH5MlgXkg==
            64: y :: n :: n :: n :: n
            80: 14000 :: -1 :: -1 :: -1 :: -1
            81: 0 :: 9 :: 9 :: 9 :: 9
             */

            byte[] decoded = System.Convert.FromBase64String(dictspec2min[52]);

            //byte[] unzipped = OptionsDecoder.DecodeZLib(decoded);
            //var text = System.Text.Encoding.ASCII.GetString(unzipped).Replace("\"", "");
            //var t2 = System.Text.Encoding.ASCII.GetString(System.Convert.FromBase64String(text));


            var dict = OptionsDecoder.DecodedToDict(OptionsDecoder.DecodeOptions("eNpFU9tymzAQ/Zd8AfjCTB/6AJFsKx1JgUgw+C3GLbWwjSeJC+jru9IKh6dlr+ecXT2pPE/hW3DV23QAi6QJT4PP3O6ZMwkdID4+bDL7u0kaPmAdTQ6tr1sJe7sTH68HaWjECVtwoiNuNHMJQnUJN3zihn34f8PD/8uf4rKxx+eseq/Gz7o6nptTRvfV/ny4ijOj4vNYsbaguhWkuXPEvpLqdkcMecKzT+eLAfvrsSx0sduIIi4/9qq4/1p8ycO2PL1vN9mrighi6S1y0aM0TZSlHnckrx11plT1Ny/VD4F3LE/RiLntQl5WFHu1CcfcBfRaIqZm1jMWpp9C/3juL0yd/M418jB04il39lqa1GvFFexDOU1nPKBVivmC9F881HLrbJwtbB+w1dBThzldIjC+5GTeZQOcaeBEYZd6wDpqwb/B+XkibLpFWwMWFocc4Egp+ptEIsdRQu9nH28m15uTdO7pOGG+AU6Ie4K7WgR9YP+gj8fCYH49ZK3v47BMIuQD52nOF4+bZIClDtqyJdwkcrY54Kp9rnR1JMy3j50uhbqtsa4b5z1yCzsjjTOtJLdtXgrNNvsXRcesiMo3RotN0ZVSR7oFLpN4y8hhV56cn+3q2N1mXf0486sw+2r9t7mc/4UZoAELe2zmW1kK9858XMePW4L3xAcfX/v3hpouH3Hbwt2Eu7J90LhZQ9zfjXTvWOXDzFm6fqjbCnQO+d0K7vDn038y+DrX"));

            var slotInfo = "eNrtlF1PwjAUhv0tvR5mn+hIvJlQHAqysXVjxovKiht0H9mmAYz/XVYcZkEuFm8woTfnvD3tOadP2goi9/QB0iyZh5To8Ty5DH3QaYvqtXTV5kBe4CJMYr0LOgIHCoKj0uU5MMezKrBVGZ6RvRsvH8g7oXs1xMUssNYpW9Eq04QRGZMMZjgiwwlbF+Ymwf6aVSlrvuVsOiIF7uICgw7QlwPNtKFm2FA2KLM9x4KPCG412mnDHpRWs+zBqLQGNb0h9QQHBfezvr4y4pHn9Ff5VCxyH6V3aGFqNstpVrkdU/CSKYWQ7XdfQ/DJHfJR1LagqlKNj3iifHr85oAPaswHNuIjipIibUcNkPKPADlBU0CDRoAEVZaVGh35hOh807D2z6k6TQp9vox5j5im7sgJFi/OSkS2L+M+clFMLbRBmUOD2FiyHCNfYPYWodQ145/rNJ7oN7+SaQk1LK2j304ZqcDs/D+R4Y+TgdrY4sNzt+duq26fL74AC+hCLg==";

            byte[] decoded2 = System.Convert.FromBase64String(slotInfo);
            byte[] unzipped2 = OptionsDecoder.DecodeZLib(decoded2);
            var text2 = System.Text.Encoding.ASCII.GetString(unzipped2).Replace("\"", "");
            var a= System.Text.Encoding.ASCII.GetString(System.Convert.FromBase64String(text2));


            Assert.Pass();
        }
    }
}