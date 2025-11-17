// OVERVIEW – inline redigering for bedrift
function initOverviewEditing() {
    const fagDataEl   = document.getElementById("fagområderData");
    const kompDataEl  = document.getElementById("kompetanserData");
    const underDataEl = document.getElementById("underkompetanserData");
    
    if (!fagDataEl || !kompDataEl || !underDataEl) return;

    const tables = document.querySelectorAll("table");
    if (!tables.length) return;

    tables.forEach(table => {
        const tbody = table.querySelector("tbody");
        if (!tbody) return;

        tbody.addEventListener("click", async e => {
            const editBtn = e.target.closest(".edit-btn");
            const saveBtn = e.target.closest(".save-btn");

            // Start redigering
            if (editBtn) {
                const row = editBtn.closest("tr");
                editBtn.classList.add("d-none");
                const localSaveBtn = row.querySelector(".save-btn");
                localSaveBtn.classList.remove("d-none");

                const cells   = row.querySelectorAll("td");
                const fagVal   = cells[0].textContent.trim();
                const kompVal  = cells[1].textContent.trim();
                const underVal = cells[2].textContent.trim();
                const bescVal  = cells[3].textContent.trim();

                const fagResponse = await fetch("/Home/GetFagomrader");
                const fagomrader  = await fagResponse.json();

                const fagSelect = document.createElement("select");
                fagSelect.classList.add("form-select", "form-select-sm");
                fagSelect.innerHTML = fagomrader.map(f =>
                    `<option value="${f.fagområdeId}" ${f.fagområdeNavn === fagVal ? 'selected' : ''}>${f.fagområdeNavn}</option>`
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
                if (!id) return; // ny rad håndteres i Add-kompetanse

                const cells = row.querySelectorAll("td");
                const fagSelectEl   = cells[0].querySelector("select");
                const kompSelectEl  = cells[1].querySelector("select");
                const underSelectEl = cells[2].querySelector("select");

                const fagomrade      = fagSelectEl ? fagSelectEl.selectedOptions[0].textContent.trim()   : cells[0].textContent.trim();
                const kompetanse     = kompSelectEl ? kompSelectEl.selectedOptions[0].textContent.trim() : cells[1].textContent.trim();
                const underkompetanse = underSelectEl?.selectedOptions[0]?.textContent.trim()            || cells[2].textContent.trim();
                const beskrivelse    = cells[3].querySelector("textarea")?.value || cells[3].textContent.trim();

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

// OVERVIEW – legg til ny kompetanse i oversiktstabellen

// Hjelpefunksjon for å starte redigering på en rad
function startEdit(row, id) {
    const editBtn = row.querySelector(".edit-btn");
    if (editBtn) {
        editBtn.classList.add("d-none");
        const saveBtn = row.querySelector(".save-btn");
        if (saveBtn) saveBtn.classList.remove("d-none");
        editBtn.dataset.dynamic = true;
        editBtn.click();
    }
}

// Initier legg til kompetanse
function initLeggTilKompetanse() {
    const addButtons = document.querySelectorAll(".add-row");
    if (!addButtons.length) return;

    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
    const token = tokenInput ? tokenInput.value : "";

    fetch("/Home/GetFagomrader")
        .then(res => res.json())
        .then(fagområder => {
            addButtons.forEach(btn => {
                btn.addEventListener("click", () => {
                    const wrapper = btn.closest(".mb-5") || btn.closest(".overview-wrapper") || document;
                    const table   = wrapper.querySelector("table");
                    const tbody   = table?.querySelector("tbody");
                    if (!tbody) return;

                    const existing = tbody.querySelector(".new-row");
                    if (existing) existing.remove();

                    const newRow = createNewRow();
                    tbody.appendChild(newRow);

                    const fagSelect = newRow.querySelector(".js-fagomrade");
                    loadFagomrader(fagSelect, fagområder);

                    setupEvents(newRow, token);
                });
            });
        });
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
    fagSelect.innerHTML = '<option value="">Velg fagområde</option>' +
        fagområder.map(f =>
            `<option value="${f.fagområdeId}" ${f.fagområdeId == valgtFagId ? 'selected' : ''}>${f.fagområdeNavn}</option>`
        ).join('');
}

// setup event listeners på ny rad
function setupEvents(newRow, token) {
    const fagSelect        = newRow.querySelector(".js-fagomrade");
    const kompSelect       = newRow.querySelector(".js-kompetanse");
    const underContainer   = newRow.querySelector(".js-underkompetanse-container");
    const beskrivelseInput = newRow.querySelector(".js-beskrivelse");
    const saveBtn          = newRow.querySelector(".save-btn");

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
            const res  = await fetch(`/Home/GetKompetanserByFagomradeId?fagomradeId=${fagId}`);
            const data = await res.json();
            kompSelect.innerHTML = '<option value="">Velg kompetanse</option>' +
                data.map(k => `<option value="${k.kompetanseId}">${k.kompetanseKategori}</option>`).join("");
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
            const res  = await fetch(`/Home/GetUnderkompetanserByKompetanseId?kompetanseId=${kompId}`);
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
        const fagomrade  = parseInt(fagSelect.value);
        const kompetanse = parseInt(kompSelect.value);

        const underkompetanser = Array.from(underContainer.querySelectorAll("input:checked"))
            .map(c => parseInt(c.value)) || [];

        const beskrivelse = beskrivelseInput.value;

        if (!fagomrade || !kompetanse) {
            alert("Velg fagområde og kompetanse først.");
            return;
        }

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

                const editBtn  = newRow.querySelector(".edit-btn");
                const deleteBtn = newRow.querySelector(".btn-delete");

                if (editBtn) {
                    editBtn.addEventListener("click", () => {
                        startEdit(newRow, data.id);
                    });
                }

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
