export default {
    api: "",
    problemTypes: [
        {id: "standard", title: "标准类型", createComponent: () => import('./components/problem/standard/Create').then(v => v.default)}
    ],
    problemType: {
        "standard": {}
    },
    templates: {
        "problem-standard-view": () => import('./components/problem/standard/View').then(v => v.default)
    }
}