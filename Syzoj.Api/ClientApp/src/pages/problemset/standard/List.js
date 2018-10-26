import React, { Component } from 'react'
import { request } from '../../../util'

export default class List extends Component {
    constructor(props) {
        super(props)
        this.state = { loading: true }
    }
    componentWillMount() {
        request("/api/problemset-standard/fc474d6c-0890-48e6-9305-ca8995918832/list", "GET", null).then(list => {
            this.setState({ loading: false, list: list })
        })
    }
    render() {
        if(this.state.loading) {
            return <p>Loading</p>
        } else {
            return this.state.list.map(f => <p>{JSON.stringify(f)}</p>)
        }
    }
}