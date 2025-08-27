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

function selectCompetence(value) {
    document.getElementById("selectedCompetence").value = value; // fyller input-feltet
    document.getElementById("dropdownContent").classList.remove("show"); // lukker dropdown
}