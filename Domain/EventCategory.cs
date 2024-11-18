namespace UI;

//omzetten naar byte zodat de enum klasse niet zo zwaar moet zijn,
//anders is het 32 bit(int), te veel voor enum
public enum EventCategory : byte
{
    Sport = 1, //0 is default voor eerste element, zet op 1 gemakkelijker voor gebruiker
    Music,
    Theater,
    Conference,
    Workshop,
    Networking,
    Festival,
    Webinair,
    Fundraiser,
    Tournament
    
}