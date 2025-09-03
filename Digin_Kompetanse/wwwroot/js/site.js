function myFunction() {
    document.getElementById("dropdownContent").classList.toggle("show");
}

function filterFunction() {
    const input = document.getElementById("input");
    const filter = input.value.toUpperCase();
    const div = document.getElementById("dropdownContent");
    const a = div.querySelectorAll("a");

    for (let i = 0; i < a.length; i++) {
        let txtValue = a[i].textContent || a[i].innerText;
        if (txtValue.toUpperCase().indexOf(filter) > -1) {
            a[i].style.display = "";
        } else {
            a[i].style.display = "none";
        }
    }
}

fetch('/Fagområde/GetFagområder')
    .then(response => response.json())
    .then(data => {
        console.log(data);
        // fyll dropdown eller skjema
    });