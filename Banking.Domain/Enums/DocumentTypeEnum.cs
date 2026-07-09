namespace Banking.Domain.Enums
{
    public enum DocumentTypeEnum
    {
        //https://www2.sunat.gob.pe/pdt/pdtModulos/independientes/p695/TipoDoc.htm
        
        Dni = 1,
        ForeignResidentCard = 4,
        Ruc = 6,
        Passport = 7,
        BirthCertificate = 11,
        Other = 0
    }
}
