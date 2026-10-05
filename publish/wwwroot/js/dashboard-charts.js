/*
 * Purpose: Draws the dashboard charts (Income by Month line chart, Sales per Category doughnut).
 * Authors: ST10068525 (new file, not yet committed)
 * Uses: Chart.js (MIT) https://www.chartjs.org
 */
// Dashboard charts: "Income by Month" (line) and "Sales per Category" (doughnut).
// Data is rendered into the page as JSON by Views/Home/Dashboard.cshtml.
(function () {
    "use strict";

    if (typeof Chart === "undefined") return;

    const root = document.querySelector(".viz-root");
    if (!root) return;

    const css = name => getComputedStyle(root).getPropertyValue(name).trim();

    const colors = {
        surface: css("--viz-surface"),
        textPrimary: css("--viz-text-primary"),
        textSecondary: css("--viz-text-secondary"),
        grid: css("--viz-grid"),
        axis: css("--viz-axis"),
        series: ["--series-1", "--series-2", "--series-3", "--series-4", "--series-5"].map(css),
        other: css("--series-other")
    };

    // Same money format as the rest of the app, e.g. R 4,475.00
    const moneyNumber = new Intl.NumberFormat("en-US", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    const moneyFull = { format: value => "R " + moneyNumber.format(value) };
    const moneyShort = value => {
        const abs = Math.abs(value);
        if (abs >= 1_000_000) return "R" + (value / 1_000_000).toFixed(1).replace(/\.0$/, "") + "m";
        if (abs >= 1_000) return "R" + (value / 1_000).toFixed(1).replace(/\.0$/, "") + "k";
        return "R" + value.toFixed(0);
    };

    function readJson(id) {
        const el = document.getElementById(id);
        try { return el ? JSON.parse(el.textContent) : []; } catch { return []; }
    }

    Chart.defaults.font.family = 'system-ui, -apple-system, "Segoe UI", sans-serif';
    Chart.defaults.color = colors.textSecondary;

    const tooltipStyle = {
        backgroundColor: "#0d0d0d",
        titleColor: colors.textPrimary,
        bodyColor: colors.textPrimary,
        borderColor: "rgba(255,255,255,0.15)",
        borderWidth: 1,
        padding: 10,
        displayColors: false
    };

    // ======================================================
    // INCOME BY MONTH - single-series line
    // ======================================================
    const incomeCanvas = document.getElementById("incomeChart");
    const income = readJson("incomeData");

    if (incomeCanvas && income.length) {
        // Vertical guide line at the hovered month
        const crosshair = {
            id: "crosshair",
            afterDatasetsDraw(chart) {
                const active = chart.tooltip && chart.tooltip.getActiveElements();
                if (!active || !active.length) return;

                const x = active[0].element.x;
                const { top, bottom } = chart.chartArea;
                const ctx = chart.ctx;

                ctx.save();
                ctx.beginPath();
                ctx.moveTo(x, top);
                ctx.lineTo(x, bottom);
                ctx.lineWidth = 1;
                ctx.strokeStyle = colors.axis;
                ctx.stroke();
                ctx.restore();
            }
        };

        new Chart(incomeCanvas, {
            type: "line",
            data: {
                labels: income.map(m => m.label),
                datasets: [{
                    label: "Income",
                    data: income.map(m => m.value),
                    borderColor: colors.series[0],
                    backgroundColor: colors.series[0],
                    borderWidth: 2,
                    tension: 0.25,
                    pointRadius: 4,              // 8px markers
                    pointHoverRadius: 6,
                    pointBackgroundColor: colors.series[0],
                    pointBorderColor: colors.surface,
                    pointBorderWidth: 2,         // surface ring around markers
                    fill: false
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                interaction: { mode: "index", intersect: false },   // hover anywhere in the column
                plugins: {
                    legend: { display: false },  // single series - the title names it
                    tooltip: {
                        ...tooltipStyle,
                        callbacks: {
                            label(ctx) {
                                const lines = [moneyFull.format(ctx.parsed.y)];
                                const i = ctx.dataIndex;
                                if (i > 0) {
                                    const prev = income[i - 1].value;
                                    if (prev > 0) {
                                        const change = (ctx.parsed.y - prev) / prev * 100;
                                        lines.push((change >= 0 ? "+" : "") + change.toFixed(1) + "% vs previous month");
                                    }
                                }
                                return lines;
                            }
                        }
                    }
                },
                scales: {
                    x: {
                        grid: { display: false },
                        border: { color: colors.axis },
                        ticks: { maxRotation: 0, autoSkip: true, maxTicksLimit: 8 }
                    },
                    y: {
                        beginAtZero: true,
                        grid: { color: colors.grid },
                        border: { display: false },
                        ticks: { callback: value => moneyShort(value), maxTicksLimit: 6 }
                    }
                }
            },
            plugins: [crosshair]
        });
    }

    // ======================================================
    // SALES PER CATEGORY - doughnut
    // ======================================================
    const categoryCanvas = document.getElementById("categoryChart");
    const categories = readJson("categoryData");

    if (categoryCanvas && categories.length) {
        // Up to 5 categories get their own colour; with more, show the top 4 and fold the
        // rest into "Other" (a validated order - neighbours stay distinguishable, incl. colour-blind).
        let slices = categories.slice();
        if (slices.length > 5) {
            const top = slices.slice(0, 4);
            const otherTotal = slices.slice(4).reduce((sum, c) => sum + c.value, 0);
            slices = top.concat([{ label: `Other (${categories.length - 4})`, value: otherTotal, isOther: true }]);
        }

        const sliceColors = slices.map((s, i) => s.isOther ? colors.other : colors.series[i]);
        const total = slices.reduce((sum, s) => sum + s.value, 0);
        const percent = value => total ? (value / total * 100) : 0;

        new Chart(categoryCanvas, {
            type: "doughnut",
            data: {
                labels: slices.map(s => s.label),
                datasets: [{
                    data: slices.map(s => s.value),
                    backgroundColor: sliceColors,
                    borderColor: colors.surface,   // 2px surface gap between slices
                    borderWidth: 2,
                    hoverOffset: 6
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                cutout: "62%",
                plugins: {
                    legend: { display: false },    // HTML legend below shows names + values
                    tooltip: {
                        ...tooltipStyle,
                        callbacks: {
                            label(ctx) {
                                return `${moneyFull.format(ctx.parsed)} (${percent(ctx.parsed).toFixed(1)}%)`;
                            }
                        }
                    }
                }
            }
        });

        // Legend: colour swatch + name + value + share (identity is never colour alone)
        const legend = document.getElementById("categoryLegend");
        if (legend) {
            slices.forEach((s, i) => {
                const li = document.createElement("li");

                const swatch = document.createElement("span");
                swatch.className = "swatch";
                swatch.style.backgroundColor = sliceColors[i];

                const name = document.createElement("span");
                name.className = "name";
                name.textContent = s.label;

                const value = document.createElement("span");
                value.className = "value";
                value.textContent = `${moneyShort(s.value)} · ${percent(s.value).toFixed(0)}%`;

                li.append(swatch, name, value);
                legend.appendChild(li);
            });
        }
    }
})();
