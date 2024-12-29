async function fetchOrganisations() {
    const response = await fetch('/api/Organisations');
    if (response.ok) {
        const organisations = await response.json();
        const tableBody = document.getElementById('organisationTable').querySelector('tbody');
        tableBody.innerHTML = ''; // Maak de tabel leeg voordat je nieuwe data toevoegt

        organisations.forEach(org => {
            const row = document.createElement('tr');
            row.innerHTML = `
                        <td>${org.orgName}</td>
                        <td>${org.orgDescription}</td>
                        <td>${new Date(org.foundedDate).toLocaleDateString()}</td>
                        <td>${org.contactEmail}</td>
                    `;
            tableBody.appendChild(row);
        });
    } else {
        console.error('Failed to fetch organisations');
    }
}

document.getElementById('refreshButton').addEventListener('click', fetchOrganisations);

// Haal de organisaties op bij het laden van de pagina
fetchOrganisations();