import React, { Component } from 'react'
import { request } from '../util'
import config from '../config'
import Table from 'react-bootstrap/lib/Table'
import Link from 'react-router-dom/Link'

export default class ProblemList extends Component {
    constructor(props) {
        super(props)
        this.state = { loading: true }
        request(`/api/problemset-standard/${config.defaultProblemsetId}/list`, "GET", null).then(list => {
            this.setState({ loading: false, list: list })
        })
    }
    render() {
        if(this.state.loading) {
            return <p>Loading</p>
        } else {
            return <Table striped bordered>
                <thead>
                    <tr>
                        <th>EntryId</th>
                        <th>Identifier</th>
                        <th>Title</th>
                    </tr>
                </thead>
                <tbody>
                    {this.state.list.map(f => <tr>
                        <td>{f.EntryId}</td>
                        <td><Link to={`/problem/${f.Identifier}`}>{f.Identifier}</Link></td>
                        <td>{f.Title}</td>
                    </tr>)}
                </tbody>
            </Table>
        }
    }
}