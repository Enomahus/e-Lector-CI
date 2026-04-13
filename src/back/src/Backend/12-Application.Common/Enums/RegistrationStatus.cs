namespace Application.Common.Enums;

public enum RegistrationStatus
{    
    ToBeProcessed, // Demande enregistré en attente de traitement au niveau de la commission
    Approuved, // Agent à traiter et valider la demande => Electeur créé avec son numéro
    Rejected, // Demande rejetée pour raison donnée (Dossier incomplet, defaut de certificat de nationalité, defaut de CNI)
}
