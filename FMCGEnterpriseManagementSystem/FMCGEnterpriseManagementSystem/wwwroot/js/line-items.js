/*
 * Purpose: New Quote / New Invoice: adds product lines, works out totals and checks each line before saving.
 * Authors: ST10068525 (new file, not yet committed)
 */
// Line item editor shared by the New Quote and New Invoice forms.
//
// Expects on the page:
//   <form data-line-items-form>              the form being submitted
//   <tbody id="lineItemsBody">               where rows are added
//   <button id="addItemBtn">                 adds a row
//   <script type="application/json" id="productsData">   [{ id, code, name, price }]
//   <script type="application/json" id="initialItems">   rows to restore after a failed save
//   #subtotalDisplay, #vatDisplay, #totalDisplay, #itemsError
//   #customerSelect (+ #billingAddress, #paymentTerms, #salesRepId, #salesRepName)
(function () {
    "use strict";

    const MAX_QTY = 100000;

    const form = document.querySelector("[data-line-items-form]");
    if (!form) return;

    // VAT rate and default terms come from Settings (rendered on the form)
    const parsedVat = parseFloat(form.dataset.vatRate);
    const VAT_RATE = isNaN(parsedVat) ? 0.15 : parsedVat;
    const DEFAULT_TERMS = form.dataset.defaultTerms || "";

    const tbody = document.getElementById("lineItemsBody");
    const products = readJson("productsData", []);
    const initialItems = readJson("initialItems", []);
    const itemsError = document.getElementById("itemsError");

    function readJson(id, fallback) {
        const el = document.getElementById(id);
        try { return el ? JSON.parse(el.textContent) : fallback; } catch { return fallback; }
    }

    function money(value) {
        return "R" + value.toLocaleString("en-ZA", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }

    function productOptions(selectedId) {
        let html = '<option value="">-- Select Product --</option>';
        products.forEach(p => {
            const selected = String(p.id) === String(selectedId) ? " selected" : "";
            html += `<option value="${p.id}" data-price="${p.price}"${selected}>${escapeHtml(p.code || "")} - ${escapeHtml(p.name)}</option>`;
        });
        return html;
    }

    function escapeHtml(text) {
        const div = document.createElement("div");
        div.textContent = text ?? "";
        return div.innerHTML;
    }

    // ---------- rows ----------

    function addRow(item) {
        item = item || {};
        const row = document.createElement("tr");
        row.className = "line-item-row";
        row.innerHTML = `
            <td>
                <input type="number" data-field="Quantity" class="form-control form-control-sm text-center item-qty"
                       min="1" max="${MAX_QTY}" step="1" value="${item.quantity ?? 1}" aria-label="Quantity">
                <div class="invalid-feedback"></div>
            </td>
            <td>
                <select data-field="ProductId" class="form-select form-select-sm item-product" aria-label="Product">
                    ${productOptions(item.productId)}
                </select>
                <div class="invalid-feedback"></div>
            </td>
            <td>
                <input type="number" data-field="UnitPrice" class="form-control form-control-sm text-end item-price"
                       min="0.01" step="0.01" value="${item.unitPrice ?? "0.00"}" aria-label="Unit price">
                <div class="invalid-feedback"></div>
            </td>
            <td>
                <input type="number" data-field="DiscountPercent" class="form-control form-control-sm text-center item-disc"
                       min="0" max="100" step="0.01" value="${item.discountPercent ?? 0}" aria-label="Discount percent">
                <div class="invalid-feedback"></div>
            </td>
            <td>
                <select data-field="VatCategory" class="form-select form-select-sm item-vat" aria-label="VAT category">
                    <option value="STANDARD"${item.vatCategory === "[NONE]" ? "" : " selected"}>Standard (15%)</option>
                    <option value="[NONE]"${item.vatCategory === "[NONE]" ? " selected" : ""}>None</option>
                </select>
            </td>
            <td class="text-end"><span class="line-total fw-semibold">R0.00</span></td>
            <td class="text-center">
                <button type="button" class="btn btn-sm btn-outline-danger remove-row" title="Remove item" aria-label="Remove item">
                    <i class="bi bi-trash"></i>
                </button>
            </td>`;

        tbody.appendChild(row);

        // Picking a product fills in its selling price
        row.querySelector(".item-product").addEventListener("change", function () {
            const option = this.options[this.selectedIndex];
            const price = option ? option.getAttribute("data-price") : null;
            if (price) row.querySelector(".item-price").value = Number(price).toFixed(2);
            clearError(this);
            recalc();
        });

        row.querySelectorAll("input, select").forEach(field => {
            field.addEventListener("input", () => { clearError(field); recalc(); });
        });

        row.querySelector(".remove-row").addEventListener("click", () => {
            row.remove();
            renumber();
            recalc();
        });

        renumber();
        recalc();
    }

    // MVC binds Items[0], Items[1], ... so indexes must have no gaps
    function renumber() {
        tbody.querySelectorAll(".line-item-row").forEach((row, index) => {
            row.querySelectorAll("[data-field]").forEach(field => {
                field.name = `Items[${index}].${field.dataset.field}`;
            });
        });
    }

    // ---------- totals ----------

    function recalc() {
        let subtotal = 0, vatTotal = 0;

        tbody.querySelectorAll(".line-item-row").forEach(row => {
            const qty = parseFloat(row.querySelector(".item-qty").value) || 0;
            const price = parseFloat(row.querySelector(".item-price").value) || 0;
            const disc = Math.min(Math.max(parseFloat(row.querySelector(".item-disc").value) || 0, 0), 100);
            const vatCategory = row.querySelector(".item-vat").value;

            const afterDiscount = qty * price * (1 - disc / 100);
            const vat = vatCategory === "[NONE]" ? 0 : afterDiscount * VAT_RATE;

            row.querySelector(".line-total").textContent = money(afterDiscount + vat);
            subtotal += afterDiscount;
            vatTotal += vat;
        });

        setText("subtotalDisplay", money(subtotal));
        setText("vatDisplay", money(vatTotal));
        setText("totalDisplay", money(subtotal + vatTotal));
    }

    function setText(id, text) {
        const el = document.getElementById(id);
        if (el) el.textContent = text;
    }

    // ---------- client-side validation ----------

    function showError(field, message) {
        field.classList.add("is-invalid");
        const feedback = field.parentElement.querySelector(".invalid-feedback");
        if (feedback) feedback.textContent = message;
    }

    function clearError(field) {
        field.classList.remove("is-invalid");
        const feedback = field.parentElement.querySelector(".invalid-feedback");
        if (feedback) feedback.textContent = "";
    }

    function validateRows() {
        const rows = tbody.querySelectorAll(".line-item-row");
        let valid = true;

        if (itemsError) {
            itemsError.textContent = rows.length === 0 ? "Add at least one item." : "";
            itemsError.classList.toggle("d-none", rows.length !== 0);
        }
        if (rows.length === 0) valid = false;

        rows.forEach(row => {
            const qty = row.querySelector(".item-qty");
            const product = row.querySelector(".item-product");
            const price = row.querySelector(".item-price");
            const disc = row.querySelector(".item-disc");

            const qtyValue = Number(qty.value);
            if (!Number.isInteger(qtyValue) || qtyValue < 1 || qtyValue > MAX_QTY) {
                showError(qty, "Enter a whole number from 1.");
                valid = false;
            }

            if (!product.value) {
                showError(product, "Select a product.");
                valid = false;
            }

            if (!(Number(price.value) > 0)) {
                showError(price, "Must be more than 0.");
                valid = false;
            }

            const discValue = Number(disc.value);
            if (disc.value === "" || isNaN(discValue) || discValue < 0 || discValue > 100) {
                showError(disc, "0 to 100 only.");
                valid = false;
            }
        });

        return valid;
    }

    form.addEventListener("submit", function (e) {
        if (!validateRows()) {
            e.preventDefault();
            e.stopImmediatePropagation();

            const firstError = form.querySelector(".is-invalid") || itemsError;
            if (firstError) firstError.scrollIntoView({ behavior: "smooth", block: "center" });
        }
    });

    // ---------- customer details ----------

    const customerSelect = document.getElementById("customerSelect");
    if (customerSelect) {
        customerSelect.addEventListener("change", function () {
            const selected = this.options[this.selectedIndex];
            const get = name => (selected && selected.getAttribute(name)) || "";

            const setValue = (id, value) => {
                const el = document.getElementById(id);
                if (el) {
                    el.value = value;
                    // Re-run jQuery validation on autofilled fields
                    if (window.jQuery && jQuery(el).valid) jQuery(el).valid();
                }
            };

            setValue("billingAddress", get("data-address"));
            setValue("paymentTerms", get("data-terms") || (this.value ? DEFAULT_TERMS : ""));
            setValue("salesRepId", get("data-salesrep"));
            setValue("salesRepName", get("data-salesrepname"));
        });
    }

    // ---------- start ----------

    document.getElementById("addItemBtn").addEventListener("click", () => addRow());

    if (initialItems.length > 0) {
        initialItems.forEach(addRow);
    } else {
        addRow();
    }
})();
