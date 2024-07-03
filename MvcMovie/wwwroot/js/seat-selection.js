const container = document.querySelector(".container");
const seats = document.querySelectorAll(".row .seat:not(.occupied)");
const count = document.getElementById("count");
const total = document.getElementById("total");
const movieSelect = document.getElementById("movie");
const seatIds = document.getElementById("seat-ids");

const sessionTimeout = 10 * 60 * 1000; // 10 minutes in milliseconds
let lastActivityTime = Date.now();

populateUI();

let ticketPrice = +movieSelect.value;

// Save selected movie index and price
function setMovieData(movieIndex, moviePrice) {
    sessionStorage.setItem("selectedMovieIndex", movieIndex);
    sessionStorage.setItem("selectedMoviePrice", moviePrice);
    resetSessionTimeout();
}

// Update total and count
function updateSelectedCount() {
    const selectedSeats = document.querySelectorAll(".row .seat.selected");

    const seatsIndex = [...selectedSeats].map(seat => {
        const row = seat.parentElement.getAttribute('data-row');
        const seatNumber = seat.getAttribute('data-seat');
        return `${row}${seatNumber}`;
    });

    sessionStorage.setItem("selectedSeats", JSON.stringify(seatsIndex));

    const selectedSeatsCount = selectedSeats.length;

    count.innerText = selectedSeatsCount;
    total.innerText = selectedSeatsCount * ticketPrice;

    seatIds.innerText = seatsIndex.length > 0 ? seatsIndex.join(", ") : "None";

    setMovieData(movieSelect.selectedIndex, movieSelect.value);
    resetSessionTimeout();
}

// Get data from sessionStorage and populate UI
function populateUI() {
    const selectedSeats = JSON.parse(sessionStorage.getItem("selectedSeats"));

    if (selectedSeats !== null && selectedSeats.length > 0) {
        seats.forEach(seat => {
            const row = seat.parentElement.getAttribute('data-row');
            const seatNumber = seat.getAttribute('data-seat');
            const seatId = `${row}${seatNumber}`;

            if (selectedSeats.indexOf(seatId) > -1) {
                seat.classList.add("selected");
            }
        });
    }

    const selectedMovieIndex = sessionStorage.getItem("selectedMovieIndex");

    if (selectedMovieIndex !== null) {
        movieSelect.selectedIndex = selectedMovieIndex;
    }

    resetSessionTimeout();
}

// Movie select event
movieSelect.addEventListener("change", (e) => {
    ticketPrice = +e.target.value;
    setMovieData(e.target.selectedIndex, e.target.value);
    updateSelectedCount();
});

// Seat click event
container.addEventListener("click", (e) => {
    if (
        e.target.classList.contains("seat") &&
        !e.target.classList.contains("occupied")
    ) {
        e.target.classList.toggle("selected");

        updateSelectedCount();
    }
});

// Initial count and total set
updateSelectedCount();

// Reset seat selection
function resetSeatSelection() {
    // Clear session storage
    sessionStorage.removeItem("selectedSeats");
    sessionStorage.removeItem("selectedMovieIndex");
    sessionStorage.removeItem("selectedMoviePrice");

    // Clear selected seats in UI
    seats.forEach(seat => {
        seat.classList.remove("selected");
    });

    // Reset count and total
    count.innerText = 0;
    total.innerText = 0;
    seatIds.innerText = "None";

    // Reset movie selection
    movieSelect.selectedIndex = 0;
    ticketPrice = +movieSelect.value;
}

// Reset session timeout
function resetSessionTimeout() {
    lastActivityTime = Date.now();
}

// Check for session timeout
setInterval(() => {
    if (Date.now() - lastActivityTime >= sessionTimeout) {
        resetSeatSelection();
    }
}, 100);

// Reset session timeout on page load (optional)
resetSessionTimeout();
