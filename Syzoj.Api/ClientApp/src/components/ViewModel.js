import React, { Component } from 'react'
import Config from '../config'

export default class ViewModel extends Component {
    constructor(props) {
        base(props)
        this.state = {loading: true }
    }

    componentWillMount() {
        let loader = Config.templates[this.props.model.Template]
        loader().then(component => {
            this.setState({loading: false, component: component})
        })
    }

    render() {
        if(this.state.loading) {
            return <p>Loading...</p>
        } else {
            let { model, ...cprops } = this.props
            cprops.content = this.props.model.Content
            return React.createElement(this.state.component, cprops)
        }
    }
}