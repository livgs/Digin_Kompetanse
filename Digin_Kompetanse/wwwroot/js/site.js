document.addEventListener("DOMContentLoaded", () => {
    initAdminFilters();
    initOtpLogin();
    initKompetanseForm();
    initOverviewEditing();
});


// ADMIN-DASHBOARD – filtrering

function initAdminFilters() {
    const fagSelect = document.getElementById("fagomrade");
    const kompSelect = document.getElementById("kompetanse");
    const underSelect = document.getElementById("underkompetanse");
    if (!fagSelect || !kompSelect || !underSelect) return;

    // Fagområde → kompetanse
    fagSelect.addEventListener("change", async function () {
        kompSelect.innerHTML = '<option value="">Alle</option>';
        underSelect.innerHTML = '<option value="">Alle</option>';
        if (!this.value) return;

        try {
            const res = await fetch(`/Admin/GetKompetanser?fagomrade=${encodeURIComponent(this.value)}`);
            const data = await res.json();
            data.forEach(k => {
                const opt = document.createElement("option");
                opt.value = k;
                opt.textContent = k;
                kompSelect.appendChild(opt);
            });
        } catch (err) {
            console.error(err);
        }
    });

    // Kompetanse → underkompetanse
    kompSelect.addEventListener("change", async function () {
        underSelect.innerHTML = '<option value="">Alle</option>';
        if (!this.value) return;

        try {
            const res = await fetch(`/Admin/GetUnderkompetanser?kompetanse=${encodeURIComponent(this.value)}`);
            const data = await res.json();
            data.forEach(u => {
                const opt = document.createElement("option");
                opt.value = u;
                opt.textContent = u;
                underSelect.appendChild(opt);
            });
        } catch (err) {
            console.error(err);
        }
    });
}


// BEDRIFT-INNLOGGING (OTP)

function initOtpLogin() {
    const stepEmail = document.getElementById("step-email");
    const stepOtp = document.getElementById("step-otp");
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
        if (el.otp) el.otp.disabled = disabled;
    }

    async function postJson(url, payload) {
        const res = await fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });
        
        let data = {};
        try { data = await res.json(); } catch { /* ignore */ }

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
            el.otp.reportValidity(); // viser "Vennligst fyll ut dette feltet."
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

    // Enter for å gå videre
    el.email?.addEventListener('keydown', (e) => { if (e.key === 'Enter') el.btnSend.click(); });
    el.otp?.addEventListener('keydown',   (e) => { if (e.key === 'Enter') el.btnVerify.click(); });
}


// KOMPETANSEREGISTRERING

function initKompetanseForm() {
    const container = document.getElementById("kompetanse-container");
    const addRowBtn = document.getElementById("add-row");
    if (!container || !addRowBtn) return;

    const getKompetanserUrl = window.getKompetanserUrl || "/Home/GetKompetanser";
    const getUnderkompetanserUrl = window.getUnderkompetanserUrl || "/Home/GetUnderkompetanser";
    
    let rowIndex = document.querySelectorAll(".kompetanse-row").length || 0;

    // Legg til rad 
    addRowBtn.addEventListener("click", () => {
        const firstRow = container.querySelector(".kompetanse-row");
        if (!firstRow) return;

        const clone = firstRow.cloneNode(true);
        clone.setAttribute("data-index", rowIndex);

        // Oppdater name/id og nullstill verdier
        clone.querySelectorAll("select, textarea").forEach(el => {
            const name = el.getAttribute("name");
            const id = el.getAttribute("id");

            if (name) {
                el.name = name.replace(/\[\d+\]/, `[${rowIndex}]`);
            }
            if (id) {
                el.id = id.replace(/_\d+__/, `_${rowIndex}__`);
            }

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

        // Skjul kompetanse- og underkompetanse-grupper
        clone.querySelectorAll(".kompetanse-group,.underkompetanse-group")
            .forEach(g => g.classList.add("d-none"));

        // Reset tekst i underkompetanse-container
        const underContainer = clone.querySelector(".js-underkompetanse-container");
        if (underContainer) {
            underContainer.innerHTML = '<p class="text-muted mb-0">Velg kompetanse først.</p>';
        }

        // Nullstill valideringsfeil
        clone.querySelectorAll("span[data-valmsg-for]").forEach(span => span.textContent = "");

        container.appendChild(clone);
        rowIndex++;
    });

    // Fjern rad 
    document.addEventListener("click", e => {
        if (!e.target.matches(".js-remove-row")) return;

        const rows = container.querySelectorAll(".kompetanse-row");
        if (rows.length > 1) {
            e.target.closest(".kompetanse-row").remove();
        }
    });

    // Fagområde → kompetanse 
    document.addEventListener("change", async e => {
        if (!e.target.matches(".js-fagomrade")) return;

        const row = e.target.closest(".kompetanse-row");
        const fagId = e.target.value;

        const kompGroup = row.querySelector(".kompetanse-group");
        const kompSelect = row.querySelector(".js-kompetanse");
        const underGroup = row.querySelector(".underkompetanse-group");
        const underContainer = row.querySelector(".js-underkompetanse-container");

        // reset kompetanse og underkompetanse
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
            console.error(err);
            alert("Kunne ikke hente kompetanser.");
        }
    });

    // Kompetanse → underkompetanse (chips)
    document.addEventListener("change", async e => {
        if (!e.target.matches(".js-kompetanse")) return;

        const row = e.target.closest(".kompetanse-row");
        const rowIdx = row.getAttribute("data-index");
        const kompetanseId = e.target.value;

        const underGroup = row.querySelector(".underkompetanse-group");
        const underContainer = row.querySelector(".js-underkompetanse-container");

        underContainer.innerHTML = "";

        if (!kompetanseId) {
            underGroup.classList.add("d-none");
            underContainer.innerHTML = '<p class="text-muted mb-0">Velg kompetanse først.</p>';
            return;
        }

        try {
            const res = await fetch(`${getUnderkompetanserUrl}?kompetanseId=${encodeURIComponent(kompetanseId)}`);
            if (!res.ok) throw new Error("Kunne ikke hente underkompetanser");

            const data = await res.json();

            if (!data.length) {
                underContainer.innerHTML = '<p class="text-muted mb-0">Ingen underkompetanser registrert.</p>';
            } else {
                // VIKTIG: riktig name/id slik at MVC binder til Rader[i].UnderkompetanseId
                underContainer.innerHTML = data.map(u => {
                    const inputId = `Rader_${rowIdx}__UnderkompetanseId_${u.underkompetanseId}`;
                    const name = `Rader[${rowIdx}].UnderkompetanseId`;

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
            console.error(err);
            alert("Kunne ikke hente underkompetanser.");
            underGroup.classList.add("d-none");
        }
    });
}


// OVERVIEW – inline redigering

function initOverviewEditing() {
    const tables = document.querySelectorAll("table");
    if (!tables.length) return;

    tables.forEach(table => {
        table.addEventListener("click", async e => {
            const editBtn = e.target.closest(".edit-btn");
            const saveBtn = e.target.closest(".save-btn");

            // === START REDIGERING ===
            if (editBtn) {
                const row = editBtn.closest("tr");
                editBtn.classList.add("d-none");
                row.querySelector(".save-btn").classList.remove("d-none");

                const id = editBtn.dataset.id;
                const cells = row.querySelectorAll("td");

                const fagomradeVal = cells[0].textContent.trim();
                const kompetanseVal = cells[1].textContent.trim();
                const underVal = cells[2].textContent.trim();
                const beskrivelseVal = cells[3].textContent.trim();

                // Hent fagområder
                const fagResponse = await fetch("/Home/GetFagomrader");
                const fagomrader = await fagResponse.json();

                // Lag dropdown for fagområde
                const fagSelect = document.createElement("select");
                fagSelect.classList.add("form-select", "form-select-sm");
                fagSelect.innerHTML = fagomrader.map(f =>
                    `<option value="${f}" ${f === fagomradeVal ? 'selected' : ''}>${f}</option>`).join('');

                cells[0].innerHTML = "";
                cells[0].appendChild(fagSelect);

                // Kompetanse dropdown
                const kompSelect = document.createElement("select");
                kompSelect.classList.add("form-select", "form-select-sm");
                cells[1].innerHTML = "";
                cells[1].appendChild(kompSelect);

                // Underkompetanse dropdown
                const underSelect = document.createElement("select");
                underSelect.classList.add("form-select", "form-select-sm");
                cells[2].innerHTML = "";
                cells[2].appendChild(underSelect);

                async function loadKompetanser(fag) {
                    const res = await fetch(`/Home/GetKompetanserByFagomrade?fagomrade=${encodeURIComponent(fag)}`);
                    const kompetanser = await res.json();
                    kompSelect.innerHTML = kompetanser.map(k =>
                        `<option value="${k}" ${k === kompetanseVal ? 'selected' : ''}>${k}</option>`).join('');
                    await loadUnderkompetanser(kompSelect.value);
                }

                async function loadUnderkompetanser(kompetanse) {
                    const res = await fetch(`/Home/GetUnderkompetanserByKompetanse?kompetanse=${encodeURIComponent(kompetanse)}`);
                    const underkompetanser = await res.json();
                    underSelect.innerHTML = underkompetanser.map(u =>
                        `<option value="${u}" ${u === underVal ? 'selected' : ''}>${u}</option>`).join('');
                }


                fagSelect.addEventListener("change", async () => {
                    await loadKompetanser(fagSelect.value);
                });

                kompSelect.addEventListener("change", async () => {
                    await loadUnderkompetanser(kompSelect.value);
                });

                await loadKompetanser(fagSelect.value);

                cells[3].innerHTML = `<textarea class="form-control form-control-sm">${beskrivelseVal}</textarea>`;
            }

            // Lagre endringer
            if (saveBtn) {
                const row = saveBtn.closest("tr");
                const id = saveBtn.getAttribute("data-id");
                const cells = row.querySelectorAll("td");

                const fagomrade = cells[0].querySelector("select").value;
                const kompetanse = cells[1].querySelector("select").value;
                const underkompetanse = cells[2].querySelector("select").value;
                const beskrivelse = cells[3].querySelector("textarea").value;

                const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
                const token = tokenInput ? tokenInput.value : "";

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

// ADMIN-DASHBOARD – filtrering
function initAdminFilters() {
    const fagSelect = document.getElementById("fagomrade");
    const kompSelect = document.getElementById("kompetanse");
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

    // === Fagområde → Kompetanse ===
    fagSelect.addEventListener("change", async function () {
        const selectedFag = this.value;

        // Reset begge undernivåer
        setLoading(kompSelect);
        setLoading(underSelect);

        if (!selectedFag) {
            resetSelect(kompSelect);
            resetSelect(underSelect);
            return;
        }

        try {
            const res = await fetch(`/Admin/GetKompetanserByFagomrade?fagomradeNavn=${encodeURIComponent(selectedFag)}`);
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

            // Fyll kompetanseliste
            data.forEach(k => {
                const opt = document.createElement("option");
                opt.value = k.kompetanseKategori;
                opt.textContent = k.kompetanseKategori;
                kompSelect.appendChild(opt);
            });

            const urlParams = getParams();
            const valgtKompetanse = urlParams.get("kompetanse");
            const valgtUnder = urlParams.get("underkompetanse");

            if (valgtKompetanse) {
                kompSelect.value = valgtKompetanse;

                // Last underkompetanser når kompetanselisten er satt
                setTimeout(() => {
                    kompSelect.dispatchEvent(new Event("change", { bubbles: true }));
                }, 100);

                // Gjenopprett valgt underkompetanse
                setTimeout(() => {
                    if (valgtUnder) underSelect.value = valgtUnder;
                }, 400);
            }
        } catch (err) {
            console.error(err);
            resetSelect(kompSelect, "Feil – prøv igjen");
            resetSelect(underSelect);
        }
    });

    // === Kompetanse → Underkompetanse ===
    kompSelect.addEventListener("change", async function () {
        const selectedKomp = this.value;
        resetSelect(underSelect);
        if (!selectedKomp) return;

        await loadUnderkompetanser(selectedKomp);
    });

    // === Hjelpefunksjon for å hente underkompetanser ===
    async function loadUnderkompetanser(kompetanseNavn, valgtUnder = null) {
        setLoading(underSelect);

        try {
            const res = await fetch(`/Admin/GetUnderkompetanserByKompetanse?kompetanseNavn=${encodeURIComponent(kompetanseNavn)}`);
            if (!res.ok) throw new Error("Kunne ikke hente underkompetanser");
            const data = await res.json();

            resetSelect(underSelect);

            if (!data.length) {
                underSelect.innerHTML = `<option value="">Ingen underkompetanser</option>`;
                underSelect.disabled = true;
                return;
            }

            data.forEach(uk => {
                const opt = document.createElement("option");
                opt.value = uk.underkompetanseNavn;
                opt.textContent = uk.underkompetanseNavn;
                underSelect.appendChild(opt);
            });

            if (valgtUnder) underSelect.value = valgtUnder;
        } catch (err) {
            console.error(err);
            resetSelect(underSelect, "Feil – prøv igjen");
        }
    }

    // === Gjenopprett ved reload ===
    const urlParams = getParams();
    const valgtFag = urlParams.get("fagomrade");

    if (valgtFag) {
        fagSelect.value = valgtFag;
        fagSelect.dispatchEvent(new Event("change"));
    }
}
