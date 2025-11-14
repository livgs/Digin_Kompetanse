document.addEventListener("DOMContentLoaded", () => {
    initAdminFilters();
    initOtpLogin();
    initKompetanseForm();
    initOverviewEditing();
    initLeggTilKompetanse();
});

//
// ADMIN-DASHBOARD – filtrering (øverst på admin-siden)
//
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
            // RIKTIG controller + param-navn
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

//
// BEDRIFT-INNLOGGING (OTP)
//
function initOtpLogin() {
    const stepEmail = document.getElementById("step-email");
    const stepOtp   = document.getElementById("step-otp");
    if (!stepEmail || !stepOtp) return;

    const REQUEST_URL = '/auth/request-otp';
    const VERIFY_URL  = '/auth/verify-otp';

    const el = {
        stepEmail:  document.getElementById('step-email'),
        stepOtp:    document.getElementById('step-otp'),
        email:      document.getElementById('email'),
        otp:        document.getElementById('otp'),
        btnSend:    document.getElementById('btnSend'),
        btnVerify:  document.getElementById('btnVerify'),
        btnBack:    document.getElementById('btnBack'),
        btnResend:  document.getElementById('btnResend'),
        message:    document.getElementById('message')
    };

    function setMsg(text, type = 'muted') {
        if (!el.message) return;
        el.message.innerHTML = `<span class="text-${type}">${text}</span>`;
    }

    function toggle(disabled) {
        [el.btnSend, el.btnVerify, el.btnResend, el.btnBack].forEach(b => b && (b.disabled = disabled));
        if (el.email) el.email.disabled = disabled;
        if (el.otp)   el.otp.disabled   = disabled;
    }

    async function postJson(url, payload) {
        const res = await fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        let data = {};
        try { data = await res.json(); } catch { /* ignorér */ }

        return { ok: res.ok, status: res.status, data };
    }

    // Steg 1: be om kode
    el.btnSend?.addEventListener('click', async () => {
        const email = el.email.value.trim();
        if (!email) {
            el.email.reportValidity();
            el.email.focus();
            return;
        }

        toggle(true);
        setMsg('Sender kode …');

        const { ok, data, status } = await postJson(REQUEST_URL, { email });
        toggle(false);

        if (ok) {
            setMsg(data.message ?? 'Vi har sendt en engangskode til e-posten din.', 'success');
            el.stepEmail.classList.add('d-none');
            el.stepOtp.classList.remove('d-none');
            el.otp.focus();
        } else {
            const msg = data.message || (status === 404 ? 'E-post ikke registrert.' : 'Kunne ikke sende kode.');
            setMsg(msg, 'danger');
        }
    });

    // Steg 2: verifiser kode
    el.btnVerify?.addEventListener('click', async () => {
        const email = el.email.value.trim();
        const code  = el.otp.value.trim();

        if (!code) {
            el.otp.reportValidity();
            el.otp.focus();
            return;
        }

        toggle(true);
        setMsg('Verifiserer kode …');

        const { ok, data, status } = await postJson(VERIFY_URL, { email, code });
        toggle(false);

        if (ok) {
            window.location.href = '/Home/Overview';
        } else {
            const msg = data.message || (status === 400 ? 'Feil eller utløpt kode.' : 'Noe gikk galt under verifisering.');
            setMsg(msg, 'danger');
        }
    });

    // Send ny kode i steg 2
    el.btnResend?.addEventListener('click', async () => {
        const email = el.email.value.trim();
        if (!email) {
            el.email.reportValidity();
            el.email.focus();
            return;
        }

        toggle(true);
        setMsg('Sender ny kode …');

        const { ok, data } = await postJson(REQUEST_URL, { email });
        toggle(false);

        setMsg(
            ok ? (data.message ?? 'Ny kode sendt.') : (data.message ?? 'Kunne ikke sende ny kode.'),
            ok ? 'success' : 'danger'
        );
    });

    // Tilbake til e-poststeg
    el.btnBack?.addEventListener('click', () => {
        el.stepOtp.classList.add('d-none');
        el.stepEmail.classList.remove('d-none');
        setMsg('');
        el.email.focus();
    });

    // Enter-handling
    el.email?.addEventListener('keydown', e => { if (e.key === 'Enter') el.btnSend.click(); });
    el.otp?.addEventListener('keydown',   e => { if (e.key === 'Enter') el.btnVerify.click(); });
}

//
// KOMPETANSEREGISTRERING (bedrift-skjema)
//
function initKompetanseForm() {
    const container = document.getElementById("kompetanse-container");
    const addRowBtn = document.getElementById("add-row");
    if (!container || !addRowBtn) return;

    const getKompetanserUrl      = window.getKompetanserUrl      || "/Home/GetKompetanser";
    const getUnderkompetanserUrl = window.getUnderkompetanserUrl || "/Home/GetUnderkompetanser";

    let rowIndex = document.querySelectorAll(".kompetanse-row").length || 0;

    // Legg til rad
    addRowBtn.addEventListener("click", () => {
        const firstRow = container.querySelector(".kompetanse-row");
        if (!firstRow) return;

        const clone = firstRow.cloneNode(true);
        clone.setAttribute("data-index", rowIndex);

        clone.querySelectorAll("select, textarea").forEach(el => {
            const name = el.getAttribute("name");
            const id   = el.getAttribute("id");

            if (name) el.name = name.replace(/\[\d+\]/, `[${rowIndex}]`);
            if (id)   el.id   = id.replace(/_\d+__/, `_${rowIndex}__`);

            if (el.tagName === "SELECT") {
                el.selectedIndex = 0;
                if (el.classList.contains("js-kompetanse")) {
                    el.innerHTML = '<option value="">Velg et fagområde først.</option>';
                    el.disabled = true;
                }
            } else if (el.tagName === "TEXTAREA") {
                el.value = "";
            }
        });

        clone.querySelectorAll(".kompetanse-group,.underkompetanse-group")
            .forEach(g => g.classList.add("d-none"));

        const underContainer = clone.querySelector(".js-underkompetanse-container");
        if (underContainer) {
            underContainer.innerHTML = '<p class="text-muted mb-0">Velg kompetanse først.</p>';
        }

        clone.querySelectorAll("span[data-valmsg-for]").forEach(span => span.textContent = "");

        container.appendChild(clone);
        rowIndex++;
    });

    // Fjern rad
    document.addEventListener("click", e => {
        if (!e.target.matches(".js-remove-row")) return;
        const rows = container.querySelectorAll(".kompetanse-row");
        if (rows.length > 1) e.target.closest(".kompetanse-row").remove();
    });

    // Fagområde → kompetanse
    document.addEventListener("change", async e => {
        if (!e.target.matches(".js-fagomrade")) return;

        const row  = e.target.closest(".kompetanse-row");
        const fagId = e.target.value;

        const kompGroup      = row.querySelector(".kompetanse-group");
        const kompSelect     = row.querySelector(".js-kompetanse");
        const underGroup     = row.querySelector(".underkompetanse-group");
        const underContainer = row.querySelector(".js-underkompetanse-container");

        if (kompSelect) {
            kompSelect.innerHTML = '<option value="">Velg et fagområde først.</option>';
            kompSelect.disabled = true;
        }
        if (kompGroup) kompGroup.classList.add("d-none");

        if (underGroup && underContainer) {
            underGroup.classList.add("d-none");
            underContainer.innerHTML = '<p class="text-muted mb-0">Velg kompetanse først.</p>';
        }

        if (!fagId) return;

        try {
            const res = await fetch(`${getKompetanserUrl}?fagområdeId=${encodeURIComponent(fagId)}`);
            if (!res.ok) throw new Error("Kunne ikke hente kompetanser");

            const data = await res.json();
            kompSelect.innerHTML = '<option value="">Velg kompetanse</option>';
            data.forEach(k => {
                const opt = document.createElement("option");
                opt.value = k.kompetanseId;
                opt.textContent = k.kompetanseKategori;
                kompSelect.appendChild(opt);
            });

            kompSelect.disabled = false;
            if (kompGroup) kompGroup.classList.remove("d-none");
        } catch (err) {
            alert("Kunne ikke hente kompetanser.");
        }
    });

    // Kompetanse → underkompetanse (chips)
    document.addEventListener("change", async e => {
        if (!e.target.matches(".js-kompetanse")) return;

        const row      = e.target.closest(".kompetanse-row");
        const rowIdx   = row.getAttribute("data-index");
        const kompId   = e.target.value;
        const underGroup     = row.querySelector(".underkompetanse-group");
        const underContainer = row.querySelector(".js-underkompetanse-container");

        underContainer.innerHTML = "";

        if (!kompId) {
            underGroup.classList.add("d-none");
            underContainer.innerHTML = '<p class="text-muted mb-0">Velg kompetanse først.</p>';
            return;
        }

        try {
            const res = await fetch(`${getUnderkompetanserUrl}?kompetanseId=${encodeURIComponent(kompId)}`);
            if (!res.ok) throw new Error("Kunne ikke hente underkompetanser");

            const data = await res.json();

            if (!data.length) {
                underContainer.innerHTML = '<p class="text-muted mb-0">Ingen underkompetanser registrert.</p>';
            } else {
                underContainer.innerHTML = data.map(u => {
                    const inputId = `Rader_${rowIdx}__UnderkompetanseId_${u.underkompetanseId}`;
                    const name    = `Rader[${rowIdx}].UnderkompetanseId`;

                    return `
                        <div class="tag-check">
                            <input type="checkbox"
                                   class="tag-check-input"
                                   name="${name}"
                                   value="${u.underkompetanseId}"
                                   id="${inputId}">
                            <label for="${inputId}" class="tag-check-label">
                                ${u.underkompetanseNavn}
                            </label>
                        </div>`;
                }).join("");
            }

            underGroup.classList.remove("d-none");
        } catch (err) {
            alert("Kunne ikke hente underkompetanser.");
            underGroup.classList.add("d-none");
        }
    });
}

//
// OVERVIEW – inline redigering for bedrift
//
function initOverviewEditing() {
    const fagDataEl = document.getElementById("fagområderData");
    const kompDataEl = document.getElementById("kompetanserData");
    const underDataEl = document.getElementById("underkompetanserData");

    // Sjekk at elementene finnes
    if (!fagDataEl || !kompDataEl || !underDataEl) return;

    // Hent pre-rendered data fra DOM
    const fagområder = JSON.parse(fagDataEl.dataset.fagområder);
    const kompetanser = JSON.parse(kompDataEl.dataset.kompetanser);
    const underkompetanser = JSON.parse(underDataEl.dataset.underkompetanser);

    const tables = document.querySelectorAll("table");
    if (!tables.length) return;

    tables.forEach(table => {
        const tbody = table.querySelector("tbody");
        tbody.addEventListener("click", async e => {
            const editBtn = e.target.closest(".edit-btn");
            const saveBtn = e.target.closest(".save-btn");

            // Start redigering
            if (editBtn) {
                const row  = editBtn.closest("tr");
                editBtn.classList.add("d-none");
                const saveBtn = row.querySelector(".save-btn");
                saveBtn.classList.remove("d-none");

                const cells = row.querySelectorAll("td");
                const fagVal   = cells[0].textContent.trim();
                const kompVal  = cells[1].textContent.trim();
                const underVal = cells[2].textContent.trim();
                const bescVal  = cells[3].textContent.trim();

                // Hvis ny rad, hent dynamiske data fra server eller JSON
                const fagområderData = JSON.parse(document.getElementById("fagområderData").dataset.fagområder);
                const fagObj = fagområder.find(f => f.fagområdeNavn === fagVal);
                const fagId = fagObj ? fagObj.fagområdeId : null;

                const fagResponse = await fetch("/Home/GetFagomrader");
                const fagomrader  = await fagResponse.json();

                const fagSelect = document.createElement("select");
                fagSelect.classList.add("form-select", "form-select-sm");
                fagSelect.innerHTML = fagomrader.map(f =>
                    `<option value="${f.fagområdeId}" ${f.fagområdeId == fagVal ? 'selected' : ''}>${f.fagområdeNavn}</option>`
                ).join('');
                
                cells[0].innerHTML = "";
                cells[0].appendChild(fagSelect);

                const kompSelect  = document.createElement("select");
                kompSelect.classList.add("form-select", "form-select-sm");
                cells[1].innerHTML = "";
                cells[1].appendChild(kompSelect);

                const underSelect = document.createElement("select");
                underSelect.classList.add("form-select", "form-select-sm");
                cells[2].innerHTML = "";
                cells[2].appendChild(underSelect);

                async function loadKompetanser(fagId) {
                    if (!fagId) return;
                    const res = await fetch(`/Home/GetKompetanserByFagomradeId?fagomradeId=${fagId}`);
                    const kompetanser = await res.json();
                    kompSelect.innerHTML = kompetanser.map(k =>
                        `<option value="${k.kompetanseId}" ${k.kompetanseKategori === kompVal ? 'selected' : ''}>${k.kompetanseKategori}</option>`
                    ).join('');
                    await loadUnderkompetanser(kompSelect.value);
                }

                async function loadUnderkompetanser(kompetanseId) {
                    if (!kompetanseId) return;
                    const res = await fetch(`/Home/GetUnderkompetanserByKompetanseId?kompetanseId=${kompetanseId}`);
                    const underkompetanser = await res.json();
                    underSelect.innerHTML = underkompetanser.map(u =>
                        `<option value="${u.underkompetanseId}" ${u.underkompetanseNavn === underVal ? 'selected' : ''}>${u.underkompetanseNavn}</option>`
                    ).join('');
                }


                fagSelect.addEventListener("change", () => loadKompetanser(fagSelect.value));

                kompSelect.addEventListener("change", async () => {
                    await loadUnderkompetanser(kompSelect.value);
                });

                await loadKompetanser(fagSelect.value);

                cells[3].innerHTML = `<textarea class="form-control form-control-sm">${bescVal}</textarea>`;
            }

            // Lagre endringer
            if (saveBtn) {
                const row   = saveBtn.closest("tr");
                const id    = saveBtn.getAttribute("data-id");
                if (!id) {
                    // Hvis raden er ny (ingen data-id), ignorer inline-save
                    return;
                }
                                
                const cells = row.querySelectorAll("td");
                const fagSelectEl = cells[0].querySelector("select");
                const kompSelectEl = cells[1].querySelector("select");
                const underSelectEl = cells[2].querySelector("select");

                const fagomrade = fagSelectEl ? fagSelectEl.selectedOptions[0].textContent.trim() : cells[0].textContent.trim();
                const kompetanse = kompSelectEl ? kompSelectEl.selectedOptions[0].textContent.trim() : cells[1].textContent.trim();
                const underkompetanse = underSelectEl?.selectedOptions[0]?.textContent.trim() || cells[2].textContent.trim();

                const beskrivelse = cells[3].querySelector("textarea")?.value || cells[3].textContent.trim();

                const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
                const token      = tokenInput ? tokenInput.value : "";


                const response = await fetch(`/Home/EditInline/${id}`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "RequestVerificationToken": token
                    },
                    body: JSON.stringify({
                        fagomrade,
                        kompetanse,
                        underkompetanse,
                        beskrivelse
                    })
                });

                if (response.ok) {
                    cells[0].textContent = fagomrade;
                    cells[1].textContent = kompetanse;
                    cells[2].textContent = underkompetanse || "Ingen";
                    cells[3].textContent = beskrivelse;

                    saveBtn.classList.add("d-none");
                    row.querySelector(".edit-btn").classList.remove("d-none");
                } else {
                    alert("Feil ved lagring!");
                }
            }
        });
    });
}


//
// OVERVIEW – legg til ny kompetanse i oversiktstabellen
//

// Hjelpefunksjon for å starte redigering på en rad
function startEdit(row, id) {
    const fagområder = JSON.parse(document.getElementById("fagområderData").dataset.fagområder);
    const kompetanser = JSON.parse(document.getElementById("kompetanserData").dataset.kompetanser);
    const underkompetanser = JSON.parse(document.getElementById("underkompetanserData").dataset.underkompetanser);

    // Finn edit-knappen i raden og simuler klikk
    const editBtn = row.querySelector(".edit-btn");
    if (editBtn) {
        editBtn.classList.add("d-none");
        const saveBtn = row.querySelector(".save-btn");
        if (saveBtn) saveBtn.classList.remove("d-none");

        // Bare trigger click hvis knappen finnes
        editBtn.dataset.dynamic = true;
        editBtn.click();
    }
}

// Initier legg til kompetanse
function initLeggTilKompetanse() {
    const addButtons = document.querySelectorAll(".add-row");
    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
    const token = tokenInput ? tokenInput.value : "";

    // Hent fagområder én gang
    fetch("/Home/GetFagomrader")
        .then(res => res.json())
        .then(fagområder => {
            addButtons.forEach(btn => {
                btn.addEventListener("click", () => {
                    const table = btn.closest(".mb-5").querySelector("table");
                    const tbody = table.querySelector("tbody");

                    // Fjern eksisterende ny rad hvis noen
                    const existing = tbody.querySelector(".new-row");
                    if (existing) existing.remove();

                    const newRow = createNewRow();
                    tbody.appendChild(newRow);

                    const fagSelect = newRow.querySelector(".js-fagomrade");
                    loadFagomrader(fagSelect, fagområder);

                    setupEvents(newRow, token);
                });
            });
        })
}

function createNewRow() {
    const newRow = document.createElement("tr");
    newRow.classList.add("new-row", "table-success");

    newRow.innerHTML = `
        <td>
            <select class="form-select form-select-sm js-fagomrade">
                <option value="">Velg fagområde</option>
            </select>
        </td>
        <td>
            <select class="form-select form-select-sm js-kompetanse" disabled>
                <option>Velg fagområde først</option>
            </select>
        </td>
        <td>
            <div class="js-underkompetanse-container text-muted">Velg kompetanse først</div>
        </td>
        <td>
            <textarea class="form-control form-control-sm js-beskrivelse" rows="1" placeholder="Beskrivelse (valgfritt)"></textarea>
        </td>
        <td>${new Date().toLocaleDateString("nb-NO")}</td>
        <td>
           <button type="button" class="btn btn-success btn-sm me-1 save-btn" title="Lagre">
                <i class="bi bi-check"></i>
            </button>
            <button type="button" class="btn btn-primary btn-sm me-1 edit-btn d-none" title="Rediger">
                <i class="bi bi-pencil-fill"></i>
            </button>
            <button type="button" class="btn btn-danger btn-sm me-1 js-cancel-new" title="Avbryt">
                <i class="bi bi-trash-fill"></i>
            </button>
        </td>

    `;
    return newRow;
}

// Fyll fagområder
function loadFagomrader(fagSelect, fagområder, valgtFagId = null) {
    fagSelect.innerHTML = fagområder.map(f =>
        `<option value="${f.fagområdeId}" ${f.fagområdeId == valgtFagId ? 'selected' : ''}>${f.fagområdeNavn}</option>`
    ).join('');
}


// setup event listeners på ny rad
function setupEvents(newRow, token) {
    const fagSelect = newRow.querySelector(".js-fagomrade");
    const kompSelect = newRow.querySelector(".js-kompetanse");
    const underContainer = newRow.querySelector(".js-underkompetanse-container");
    const beskrivelseInput = newRow.querySelector(".js-beskrivelse");
    const saveBtn = newRow.querySelector(".save-btn");
    const editBtn = newRow.querySelector(".edit-btn");

    // Når fagområde velges
    fagSelect.addEventListener("change", async () => {
        const fagId = fagSelect.value;
        kompSelect.disabled = true;
        kompSelect.innerHTML = '<option>Laster…</option>';
        underContainer.innerHTML = 'Velg kompetanse først';

        if (!fagId) {
            kompSelect.innerHTML = '<option>Velg fagområde først</option>';
            return;
        }

        try {
            const res = await fetch(`/Home/GetKompetanserByFagomradeId?fagomradeId=${fagId}`);
            const data = await res.json();
            kompSelect.innerHTML = '<option value="">Velg kompetanse</option>' +
                data.map(k => `<option value="${k.kompetanseId}">${k.kompetanseKategori}</option>`)
            kompSelect.disabled = false;
        } catch (err) {
            kompSelect.innerHTML = '<option>Feil – prøv igjen</option>';
            kompSelect.disabled = true;
        }
    });

    // Når kompetanse velges
    kompSelect.addEventListener("change", async () => {
        const kompId = kompSelect.value;
        underContainer.innerHTML = 'Laster…';

        if (!kompId) {
            underContainer.innerHTML = 'Velg kompetanse først';
            return;
        }

        try {
            const res = await fetch(`/Home/GetUnderkompetanserByKompetanseId?kompetanseId=${kompId}`);
            const data = await res.json();
            if (!data.length) {
                underContainer.innerHTML = 'Ingen underkompetanser registrert';
            } else {
                underContainer.innerHTML = data.map(u => `
                  <div class="tag-check">
                    <input type="checkbox" name="UnderkompetanseId" value="${u.underkompetanseId}" id="uk_${u.underkompetanseId}">
                    <label for="uk_${u.underkompetanseId}">${u.underkompetanseNavn}</label>
                  </div>
                `).join("");
            }
        } catch (err) {
            underContainer.innerHTML = 'Feil ved henting';
        }
    });

    // Avbryt
    newRow.querySelector(".js-cancel-new").addEventListener("click", () => newRow.remove());

    // Lagre ny rad
    saveBtn.addEventListener("click", async () => {

        const fagomrade = parseInt(fagSelect.value);
        const kompetanse = parseInt(kompSelect.value);

        // Hent alle valgte underkompetanser, eller [] hvis ingen
        const underkompetanser = Array.from(underContainer.querySelectorAll("input:checked"))
            .map(c => parseInt(c.value)) || [];

        const beskrivelse = beskrivelseInput.value;

        if (!fagomrade || !kompetanse) {
            alert("Velg fagområde og kompetanse først.");
            return;
        }

        // SJEKKFUNKSJON START
        const payload = {
            FagområdeId: fagomrade,
            KompetanseId: kompetanse,
            UnderkompetanseId: underkompetanser,
            Beskrivelse: beskrivelse
        };
        console.log("Token:", token);
        console.log("Payload som sendes:", payload);

        try {
            const res = await fetch("/Home/AddKompetanse", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "RequestVerificationToken": token
                },
                body: JSON.stringify({
                    FagområdeId: fagomrade,
                    KompetanseId: kompetanse,
                    UnderkompetanseId: underkompetanser,
                    Beskrivelse: beskrivelse
                })
            });

            if (res.ok) {
                const data = await res.json();

                // Oppdater raden med de nye verdiene
                newRow.innerHTML = `
                <td>${data.fagomradeNavn}</td>
                <td>${data.kompetanseNavn}</td>
                <td>${data.underkompetanseNavn?.join(", ") || "Ingen"}</td>
                <td>${beskrivelse}</td>
                <td>${new Date().toLocaleDateString("nb-NO")}</td>
                <td>
                    <button type="button" class="btn btn-primary btn-sm me-1 edit-btn" data-id="${data.id}" title="Rediger">
                        <i class="bi bi-pencil-fill"></i>
                    </button>
                    <button type="button" class="btn btn-success btn-sm me-1 save-btn d-none" data-id="${data.id}" title="Lagre">
                        <i class="bi bi-check"></i>
                    </button>
                    <button type="button" class="btn btn-danger btn-sm me-1 btn-delete" title="Slett">
                        <i class="bi bi-trash-fill"></i>
                    </button>
                </td>
            `;

                newRow.classList.remove("new-row", "table-success");

                // Bind edit-knapp til startEdit
                const editBtn = newRow.querySelector(".edit-btn");
                if (editBtn) {
                    editBtn.addEventListener("click", () => {
                        startEdit(newRow, data.id);
                    });
                }

                // Bind delete-knapp
                const deleteBtn = newRow.querySelector(".btn-delete");
                if (deleteBtn) {
                    deleteBtn.addEventListener("click", () => {
                        if (confirm("Er du sikker på at du vil slette denne kompetansen?")) {
                            fetch(`/Home/Delete/${data.id}`, {
                                method: "POST",
                                headers: {"RequestVerificationToken": token}
                            }).then(res => {
                                if (res.ok) newRow.remove();
                                else alert("Kunne ikke slette raden.");
                            });
                        }
                    });
                }
            } else {
                alert("Feil ved lagring av ny kompetanse.");
            }
        } catch (err) {
            alert("Feil ved lagring av ny kompetanse.");
        }
    });
}