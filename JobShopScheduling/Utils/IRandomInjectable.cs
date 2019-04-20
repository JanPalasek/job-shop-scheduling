namespace JobShopScheduling.Utils
{
    using System;

    public interface IRandomInjectable
    {
        void InjectRandom(Random random);
    }
}