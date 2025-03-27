/**
 * async maakt de functie asynchroon wat betekent dat ik await kan gebruiken om te wachten operaties zoals API ophalen
 * Fetch() roept de API endpoint aan, hier worden de events opgehaald van een bepaalde visitor
 * await wacht dus toch fetch word aangeroepen en zal het resultaat opslaan in response
 * response.ok geeft een 200-299 code mee om aan te tonen dat de aanroep succesvol was
 * response.json convert de body van het htpp response naar JS object, events bevat nu een lijst eventen die gekoppeld zijn aan de visitor
 * vervolgens wordt er gezocht naar een html element met id eventtable, de queryselector selecteert tbody waarin een tabel met de eventementen komt te staan
 * tablebody wordt eerst leeg gegeven voordat er nieuwe gegevens in komen
 * vervolgens wordt door events geitereerd en worden de juiste html elementen gecreerd. 
 * tr voor een nieuwe rij met daarin de gegevens van het event.
 * de rij wordt uiteindelijk toegevoegd aan tablebody met appendchild
 */
async function fetchEventsForVisitor(visitorId) {
    const response = await fetch(`/api/Visitors/${visitorId}/Events`);
    if (response.ok) {
        const events = await response.json();
        const tableBody = document.getElementById('eventTable').querySelector('tbody');
        tableBody.innerHTML = ''; // Maak de tabel leeg voordat je nieuwe data toevoegt

        events.forEach(event => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${event.eventName}</td>
                <td>${new Date(event.eventDate).toLocaleDateString()}</td>
                <td>${event.category}</td>
                <td>${event.ticketPrice ? `€${event.ticketPrice.toFixed(2)}` : "Free"}</td>
            `;
            tableBody.appendChild(row);
        });
    } else {
        console.error('Failed to fetch events for the visitor.');
    }
}

async function fetchAvailableEvents(visitorId) {
    const response = await fetch(`/api/Visitors/${visitorId}/AvailableEvents`);
    if (response.ok) {
        const events = await response.json();
        const eventSelect = document.getElementById("eventSelect");
        eventSelect.innerHTML = ""; // Leeg de select-box

        events.forEach(event => {
            const option = document.createElement("option");
            option.value = event.eventId;
            option.textContent = `${event.eventName} (${new Date(event.eventDate).toLocaleDateString()})`;
            eventSelect.appendChild(option);
        });
    } else {
        console.error("Failed to fetch available events.");
    }
}

/**
 * door DOMcontentloaded te gebruiken wordt er pas code uitgevoerd wanneer alle html elementen op de pagina beschikbaar zijn
 * de waarde visitorid wordt opgehaald uit de hidden html element in de view
 */
document.addEventListener('DOMContentLoaded', () => {
    const visitorId = document.getElementById('visitorId').value;
    fetchEventsForVisitor(visitorId);
    fetchAvailableEvents(visitorId);
});

/**
 * addEventForm wordt aangesproken, er wordt een listener op toegevoegd zodat de callbackfunctie bij elke submit wordt uitgevoerd
 * de listener is async function dus we kunnen await gebruiken bij fetchen van http req
 * e.preventDefault voorkomt dat het de standaard actie uitvoer, namelijk opsturen van gegevens naar server en pagina herladen
 * vervolgens worden de nodige gegevens verzamelt om een nieuw ticket te maken
 * payload wordt gemaakt zodat gegevens naar api gestuurd kunnen worden
 * het http post verzoek wordt naar endpoint verzonden, payload wordt omgezet naar JSON string
 * als alles succesvol was worden de lijst en selectbox ververst 
 */
document.getElementById("addEventForm").addEventListener("submit", async function (e) {
    e.preventDefault();

    const visitorId = document.getElementById("visitorId").value;
    const eventId = document.getElementById("eventSelect").value;
    const paymentMethod = document.getElementById("paymentMethod").value;

    const payload = {
        eventId: eventId,
        visitorId: visitorId,
        paymentMethod: paymentMethod
    };

    const response = await fetch("/api/Tickets", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload)
    });

    if (response.ok) {
        console.log("Event added successfully.");
        fetchEventsForVisitor(visitorId); // Ververs de tabel
        fetchAvailableEvents(visitorId); // Ververs de select-box
    } else {
        console.error("Failed to add event.");
    }
});
