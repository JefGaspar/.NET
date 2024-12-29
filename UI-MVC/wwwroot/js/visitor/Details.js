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

document.addEventListener('DOMContentLoaded', () => {
    const visitorId = document.getElementById('visitorId').value;
    fetchEventsForVisitor(visitorId);
    fetchAvailableEvents(visitorId);
});

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
