
public class BuffFactory
{
    public static NewBuffBase CreateBuff(string name, UnitBase c, BuffManager manager)
    {
        NewBuffBase buff = null;
        /*if (name == "poisoned")
            buff = CreatePoisoned(c, manager);
        if (name == "hemophagia")
            buff = CreateHemophagia(c, manager);*/
        return buff;
    }
    /*private static NewBuffBase CreatePoisoned(UnitBase c, BuffManager manager)
    {
        NewBuffBase poisoned = new PoisonedBuff(manager, c);
        return poisoned;
    }
    private static NewBuffBase CreateHemophagia(UnitBase c, BuffManager manager)
    {
        NewBuffBase hemophagia = new HemophagiaBuff(manager, c);
        return hemophagia;
    }*/
}