document.addEventListener('DOMContentLoaded', function () {

    const updateButton = document.getElementById('updatePriceButton');
    if (updateButton) {
        updateButton.addEventListener('click', function () {
            const eventId = document.getElementById('eventId').value;
            const ticketPrice = document.getElementById('ticketPrice').value;
            
            // Valideer de ticketprijs
            if (!eventId || isNaN(parseFloat(ticketPrice)) || parseFloat(ticketPrice) < 0) {
                alert('Please enter a valid ticket price.');
                return;
            }

            fetch(`/api/Events/${eventId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                },
                credentials: 'same-origin', // Zorg ervoor dat de authenticatiecookie wordt meegestuurd
                body: JSON.stringify({ ticketPrice: parseFloat(ticketPrice) })
            })
                .then(response => {
                    if (!response.ok) {
                        if (response.status === 403) {
                            throw new Error('You are not authorized to update this event.');
                        } else if (response.status === 401) {
                            throw new Error('You need to be logged in to update this event.');
                        }
                        throw new Error('Failed to update ticket price.');
                    }
                    return response.json();
                })
                .then(data => {
                    // Update de UI dynamisch
                    alert('Ticket price updated successfully!');
                    document.getElementById('ticketPrice').value = data.newPrice.toFixed(2);
                })
                .catch(error => {
                    console.error('Error:', error);
                    alert(error.message);
                });
        });
    } else {
        console.log('Update button not found'); 
    }
});