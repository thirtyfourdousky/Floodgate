using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floodgate.NotEnums;
//redo this whole thing to use hashsets and load from a json instead of this shit
public static class CreatureTemplateType
{
    public static List<RegisteredCrit> RegisteredTemplates
    {
        get
        {
            return typeof(CreatureTemplateType).GetFields().Select(f => f.GetValue(null) as RegisteredCrit).Where(i=>i != null && !string.IsNullOrWhiteSpace(i.Name)).ToList();
        }
    }
    //M4rblelous Entity Pack LBMergedMods
    public static readonly RegisteredCrit Blizzor = new RegisteredCrit();
    public static readonly RegisteredCrit BouncingBall = new RegisteredCrit();
    public static readonly RegisteredCrit WaterBlob = new RegisteredCrit();
    public static readonly RegisteredCrit ChipChop = new RegisteredCrit();
    public static readonly RegisteredCrit CommonEel = new RegisteredCrit();
    public static readonly RegisteredCrit MiniLeviathan = new RegisteredCrit();
    public static readonly RegisteredCrit Denture = new RegisteredCrit();
    public static readonly RegisteredCrit DivingBeetle = new RegisteredCrit();
    public static readonly RegisteredCrit MiniBlackLeech = new RegisteredCrit();
    public static readonly RegisteredCrit FlyingBigEel = new RegisteredCrit();
    public static readonly RegisteredCrit FatFireFly = new RegisteredCrit();
    public static readonly RegisteredCrit Glowpillar = new RegisteredCrit();
    public static readonly RegisteredCrit HazerMom = new RegisteredCrit();
    public static readonly RegisteredCrit Hoverfly = new RegisteredCrit();
    public static readonly RegisteredCrit HunterSeeker = new RegisteredCrit();
    public static readonly RegisteredCrit Killerpillar = new RegisteredCrit();
    public static readonly RegisteredCrit Scutigera = new RegisteredCrit();
    public static readonly RegisteredCrit MiniFlyingBigEel = new RegisteredCrit();
    public static readonly RegisteredCrit MiniScutigera = new RegisteredCrit();
    public static readonly RegisteredCrit MoleSalamander = new RegisteredCrit();
    public static readonly RegisteredCrit NoodleEater = new RegisteredCrit();
    public static readonly RegisteredCrit Polliwog = new RegisteredCrit();
    public static readonly RegisteredCrit WaterSpitter = new RegisteredCrit();
    public static readonly RegisteredCrit RedHorrorCenti = new RegisteredCrit();
    public static readonly RegisteredCrit Sporantula = new RegisteredCrit();
    public static readonly RegisteredCrit SilverLizard = new RegisteredCrit();
    public static readonly RegisteredCrit ThornBug = new RegisteredCrit();
    public static readonly RegisteredCrit SurfaceSwimmer = new RegisteredCrit();
    public static readonly RegisteredCrit TintedBeetle = new RegisteredCrit();
    public static readonly RegisteredCrit SparkEye = new RegisteredCrit();
    public static readonly RegisteredCrit ScavengerSentinel = new RegisteredCrit();

    //Shrouded Assembly Code
    public static readonly RegisteredCrit Gecko = new RegisteredCrit();
    public static readonly RegisteredCrit MaracaSpider = new RegisteredCrit();
    public static readonly RegisteredCrit BabyCroaker = new RegisteredCrit();

    //Luminous Code
    public static readonly RegisteredCrit Teuthicada = new RegisteredCrit();

    //Moss Fields SnootShootNoot
    public static readonly RegisteredCrit SnootShootNoot = new RegisteredCrit();

    //scroungers (not the frostbite one)
    public static readonly RegisteredCrit Scrounger = new RegisteredCrit();

    //vanguard
    public static readonly RegisteredCrit ToxicSpider = new RegisteredCrit();
    public static readonly RegisteredCrit MaleParasite = new RegisteredCrit();
    public static readonly RegisteredCrit FemaleParasite = new RegisteredCrit();
    public static readonly RegisteredCrit ChildParasite = new RegisteredCrit();


    public static readonly RegisteredCrit _LBFly = new RegisteredCrit();

    public static void PostModsInit()
    {
        try
        {
            Blizzor.Set("",0);
            BouncingBall.Set("", 0);
            WaterBlob.Set("", 0);
            ChipChop.Set("", 0);
            CommonEel.Set("", 0);
            MiniLeviathan.Set("", 0);
            Denture.Set("", 0);
            DivingBeetle.Set("", 0);
            MiniBlackLeech.Set("", 0);
            FlyingBigEel.Set("", 0);
            FatFireFly.Set("", 0);
            Glowpillar.Set("", 0);
            MaracaSpider.Set("", 0);
            HazerMom.Set("", 0);
            Hoverfly.Set("", 0);
            HunterSeeker.Set("", 0);
            Gecko.Set("", 0);
            Killerpillar.Set("", 0);
            Scutigera.Set("", 0);
            MiniFlyingBigEel.Set("", 0);
            MiniScutigera.Set("", 0);
            MoleSalamander.Set("", 0);
            NoodleEater.Set("", 0);
            BabyCroaker.Set("", 0);
            Polliwog.Set("", 0);
            WaterSpitter.Set("", 0);
            RedHorrorCenti.Set("", 0);
            Sporantula.Set("", 0);
            SilverLizard.Set("", 0);
            ThornBug.Set("", 0);
            SurfaceSwimmer.Set("", 0);
            ScavengerSentinel.Set("", 0);
            SparkEye.Set("", 0);
            TintedBeetle.Set("", 0);

            Teuthicada.Set("", 0);

            SnootShootNoot.Set("", 0);

            Scrounger.Set("", 0);

            ToxicSpider.Set("", 0);
            MaleParasite.Set("", 0);
            FemaleParasite.Set("", 0);
            ChildParasite.Set("", 0);

            _LBFly.Set("", 0);


            if (EnabledMods.ShroudedAssemblySpecific)
            {
                Gecko.Set("Gecko", 5);
                MaracaSpider.Set("MaracaSpider", 7);
                BabyCroaker.Set("BabyCroaker", 2);
            }
            if (EnabledMods.LuminousCode)
            {
                Teuthicada.Set("Teuthicada", 4); //arbitrary; double from regular cicada
            }
            if (EnabledMods.SnootShootNoot)
            {
                SnootShootNoot.Set("SnootShootNoot", 15); //arvitrary; triple from regular noodlefly
            }
            if (EnabledMods.Scroungers)
            {
                Scrounger.Set("Scrounger", 9); //arbitrary; half way from regular (6) scav and elite scav (12)
            }
            if (EnabledMods.LBmergedMods)
            {
                Blizzor.Set("Blizzor", 18);
                BouncingBall.Set("BouncingBall", 2);
                ChipChop.Set("ChipChop", 3);
                CommonEel.Set("CommonEel", 12);
                Denture.Set("Denture", 0);
                DivingBeetle.Set("DivingBeetle", 6);
                FatFireFly.Set("FatFireFly", 23);
                FlyingBigEel.Set("FlyingBigEel", 25);
                Glowpillar.Set("Glowpillar", 7);
                HazerMom.Set("HazerMom", 3);
                Hoverfly.Set("Hoverfly", 2);
                HunterSeeker.Set("HunterSeeker", 9);
                Killerpillar.Set("Killerpillar", 7);
                //m4rjaws? 25
                MiniFlyingBigEel.Set("MiniFlyingBigEel", 10);
                MiniBlackLeech.Set("MiniBlackLeech", 0);
                MiniLeviathan.Set("MiniLeviathan", 10);
                MiniScutigera.Set("MiniScutigera", 2);
                MoleSalamander.Set("MoleSalamander", 7);
                NoodleEater.Set("NoodleEater", 3);
                Polliwog.Set("Polliwog", 5);
                RedHorrorCenti.Set("RedHorrorCenti", 29);
                ScavengerSentinel.Set("ScavengerSentinel", 12);
                SparkEye.Set("SparkEye", 0); //unknown value
                Scutigera.Set("Scutigera", 13);
                SilverLizard.Set("SilverLizard", 12);
                Sporantula.Set("Sporantula", 9);
                SurfaceSwimmer.Set("SurfaceSwimmer", 3);
                ThornBug.Set("ThornBug", 6);
                TintedBeetle.Set("TintedBeetle", 3);
                WaterBlob.Set("WaterBlob", 2);
                WaterSpitter.Set("WaterSpitter", 9);


                _LBFly.Set("lbfly", 0); //just needs to not be empty. peak design, i know
            }
            if (EnabledMods.Vanguard)
            {
                ToxicSpider.Set("ToxicSpider", 5);
                MaleParasite.Set("MaleParasite", 5);
                FemaleParasite.Set("FemaleParasite", 5);
                ChildParasite.Set("ChildParasite", 5);
            }
        }
        catch (Exception e)
        {
            FloodgatePatcher.CustomLog.LogError(e.ToString());
        }
    }

    public class RegisteredCrit
    {
        public string Name { get; private set; } = "";
        public int Score { get; private set; } = 0;
        
        public void Set(string name, int score)
        {
            Name = name;
            Score = score;
        }

        public static implicit operator string(RegisteredCrit self)
        {
            return self.Name;
        }
    }
}
