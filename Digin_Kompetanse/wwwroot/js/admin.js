// ADMIN-DASHBOARD 
function initAdminFilters() {
    const fagSelect   = document.getElementById("fagomrade");
    const kompSelect  = document.getElementById("kompetanse");
    const underSelect = document.getElementById("underkompetanse");

    if (!fagSelect || !kompSelect || !underSelect) return;

    const setLoading = (select, text = "Laster…") => {
        select.innerHTML = `<option>${text}</option>`;
        select.disabled = true;
    };

    const resetSelect = (select, placeholder = "Alle") => {
        select.innerHTML = `<option value="">${placeholder}</option>`;
        select.disabled = false;
    };

    const getParams = () => new URLSearchParams(window.location.search);

    // Fagområde → kompetanse
    fagSelect.addEventListener("change", async function () {
        const selectedFag = this.value;

        setLoading(kompSelect);
        setLoading(underSelect);

        if (!selectedFag) {
            resetSelect(kompSelect);
            resetSelect(underSelect);
            return;
        }

        try {
            const res = await fetch(`/Home/GetKompetanserByFagomrade?fagomrade=${encodeURIComponent(selectedFag)}`);
            if (!res.ok) throw new Error("Kunne ikke hente kompetanser");
            const data = await res.json();

            resetSelect(kompSelect);
            resetSelect(underSelect);

            if (!data.length) {
                kompSelect.innerHTML = `<option value="">Ingen kompetanser funnet</option>`;
                kompSelect.disabled = true;
                underSelect.innerHTML = `<option value="">Ingen underkompetanser</option>`;
                underSelect.disabled = true;
                return;
            }

            data.forEach(k => {
                const opt = document.createElement("option");
                opt.value = k;      // k er en streng (KompetanseKategori)
                opt.textContent = k;
                kompSelect.appendChild(opt);
            });

            const urlParams       = getParams();
            const valgtKompetanse = urlParams.get("kompetanse");
            const valgtUnder      = urlParams.get("underkompetanse");

            if (valgtKompetanse) {
                kompSelect.value = valgtKompetanse;
                setTimeout(() => {
                    kompSelect.dispatchEvent(new Event("change", { bubbles: true }));
                }, 100);
                setTimeout(() => {
                    if (valgtUnder) underSelect.value = valgtUnder;
                }, 400);
            }
        } catch (err) {
            resetSelect(kompSelect, "Feil – prøv igjen");
            resetSelect(underSelect);
        }
    });

    // Kompetanse → underkompetanse
    kompSelect.addEventListener("change", async function () {
        const selectedKomp = this.value;
        resetSelect(underSelect);
        if (!selectedKomp) return;

        await loadUnderkompetanser(selectedKomp);
    });

    async function loadUnderkompetanser(kompetanseNavn, valgtUnder = null) {
        setLoading(underSelect);

        try {
            const res = await fetch(`/Home/GetUnderkompetanserByKompetanse?kompetanse=${encodeURIComponent(kompetanseNavn)}`);
            if (!res.ok) throw new Error("Kunne ikke hente underkompetanser");
            const data = await res.json();

            resetSelect(underSelect);

            if (!data.length) {
                underSelect.innerHTML = `<option value="">Ingen underkompetanser</option>`;
                underSelect.disabled = true;
                return;
            }

            data.forEach(u => {
                const opt = document.createElement("option");
                opt.value = u;      // u er UnderkompetanseNavn (streng)
                opt.textContent = u;
                underSelect.appendChild(opt);
            });

            if (valgtUnder) underSelect.value = valgtUnder;
        } catch (err) {
            resetSelect(underSelect, "Feil – prøv igjen");
        }
    }

    // Gjenopprett ved reload
    const urlParams = getParams();
    const valgtFag  = urlParams.get("fagomrade");

    if (valgtFag) {
        fagSelect.value = valgtFag;
        fagSelect.dispatchEvent(new Event("change"));
    }
}
